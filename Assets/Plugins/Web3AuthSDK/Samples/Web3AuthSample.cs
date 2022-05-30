﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Web3AuthSample : MonoBehaviour
{
    List<LoginVerifier> verifierList = new List<LoginVerifier> {
        new LoginVerifier("Google", Provider.GOOGLE),
        new LoginVerifier("Facebook", Provider.FACEBOOK),
        new LoginVerifier("Twitch", Provider.TWITCH),
        new LoginVerifier("Discord", Provider.DISCORD),
        new LoginVerifier("Reddit", Provider.REDDIT),
        new LoginVerifier("Apple", Provider.APPLE),
        new LoginVerifier("Github", Provider.GITHUB),
        new LoginVerifier("LinkedIn", Provider.LINKEDIN),
        new LoginVerifier("Twitter", Provider.TWITTER),
        new LoginVerifier("Line", Provider.LINE),
        new LoginVerifier("Hosted Email Passwordless", Provider.EMAIL_PASSWORDLESS),
    };

    Web3Auth web3Auth;

    [SerializeField]
    InputField emailAddressField;

    [SerializeField]
    Dropdown verifierDropdown;

    [SerializeField]
    Button loginButton;

    [SerializeField]
    Text loginResponseText;

    [SerializeField]
    Button logoutButton;

    void Start()
    {
        web3Auth = new Web3Auth(new Web3AuthOptions()
        {
            redirectUrl = new Uri("torusapp://com.torus.Web3AuthUnity/auth"),
            clientId = "BAwFgL-r7wzQKmtcdiz2uHJKNZdK7gzEf2q-m55xfzSZOw8jLOyIi4AVvvzaEQO5nv2dFLEmf9LBkF8kaq3aErg",
            network = Web3Auth.Network.TESTNET,
            whiteLabel = new WhiteLabelData()
            {
                name = "Web3Auth Sample App",
                logoLight = null,
                logoDark = null,
                defaultLanguage = "en",
                dark = true,
                theme = new Dictionary<string, string>
                {
                    { "primary", "#123456" }
                }
            }
        });
        web3Auth.onLogin += onLogin;
        web3Auth.onLogout += onLogout;

        emailAddressField.gameObject.SetActive(false);
        logoutButton.gameObject.SetActive(false);

        loginButton.onClick.AddListener(login);
        logoutButton.onClick.AddListener(logout);

        verifierDropdown.AddOptions(verifierList.Select(x => x.name).ToList());
        verifierDropdown.onValueChanged.AddListener(onVerifierDropDownChange);
    }

    private void onLogin(Web3AuthResponse response)
    {
        Dispatcher.Instance().Enqueue(() =>
        {
            loginResponseText.text = JsonConvert.SerializeObject(response, Formatting.Indented);

            loginButton.gameObject.SetActive(false);
            verifierDropdown.gameObject.SetActive(false);
            logoutButton.gameObject.SetActive(true);
        });
    }

    private void onLogout()
    {
        Dispatcher.Instance().Enqueue(() =>
        {
            loginButton.gameObject.SetActive(true);
            verifierDropdown.gameObject.SetActive(true);
            logoutButton.gameObject.SetActive(false);

            loginResponseText.text = "";
        });
    }


    private void onVerifierDropDownChange(int selectedIndex)
    {
        if (verifierList[selectedIndex].loginProvider == Provider.EMAIL_PASSWORDLESS)
            emailAddressField.gameObject.SetActive(true);
        else
            emailAddressField.gameObject.SetActive(false);
    }

    private void login()
    {
        var selectedProvider = verifierList[verifierDropdown.value].loginProvider;

        var options = new LoginParams()
        {
            loginProvider = selectedProvider
        };

        if (selectedProvider == Provider.EMAIL_PASSWORDLESS)
        {
            options.extraLoginOptions = new ExtraLoginOptions()
            {
                login_hint = emailAddressField.text
            };
        }

        web3Auth.login(options);
    }

    private void logout()
    {
        web3Auth.logout();
    }
}

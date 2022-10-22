﻿using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Models;
using Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Services;

namespace Nop.Plugin.MultiFactorAuth.GoogleAuthenticator.Factories
{
    public class AuthenticationModelFactory
    {
        #region Fields

        private readonly GoogleAuthenticatorService _googleAuthenticatorService;
        private readonly GoogleAuthenticatorSettings _googleAuthenticatorSettings;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public AuthenticationModelFactory(IWorkContext workContext,
            GoogleAuthenticatorService googleAuthenticatorService,
            GoogleAuthenticatorSettings googleAuthenticatorSettings)
        {
            _workContext = workContext;
            _googleAuthenticatorService = googleAuthenticatorService;
            _googleAuthenticatorSettings = googleAuthenticatorSettings;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<AuthModel> PrepareAuthModel(AuthModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var secretkey = Guid.NewGuid().ToString().Replace("-", "")[0..10];
            var setupInfo = await _googleAuthenticatorService.GenerateSetupCode(secretkey);
            var customer = await _workContext.GetCurrentCustomerAsync();

            model.SecretKey = secretkey;
            model.Account = $"{_googleAuthenticatorSettings.BusinessPrefix} ({customer.Email})";
            model.ManualEntryQrCode = setupInfo.ManualEntryKey;
            model.QrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;

            return model;
        }

        #endregion
    }
}

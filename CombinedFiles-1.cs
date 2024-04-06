using System.Numerics;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.XUnitEthereumClients;
using Xunit;

namespace Nethereum.Accounts.IntegrationTests
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class TransferEtherTests
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public TransferEtherTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        [Fact]
        public async void ShouldTransferEtherWithGasPrice()
        {
            
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            
            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
            var balanceOriginal = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            var balanceOriginalEther = Web3.Web3.Convert.FromWei(balanceOriginal.Value);
            var receipt = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(toAddress, 1.11m, gasPriceGwei:2).ConfigureAwait(false);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(balanceOriginalEther + 1.11m, Web3.Web3.Convert.FromWei(balance));
        }

        [Fact]
        public async void ShouldTransferEther()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BA1";
            var balanceOriginal = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            var balanceOriginalEther = Web3.Web3.Convert.FromWei(balanceOriginal.Value);

            var receipt = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(toAddress, 1.11m).ConfigureAwait(false);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(balanceOriginalEther + 1.11m, Web3.Web3.Convert.FromWei(balance));
        }

        [Fact]
        public async void ShouldTransferEtherWithGasPriceAndGasAmount()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BA1";
            var balanceOriginal = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            var balanceOriginalEther = Web3.Web3.Convert.FromWei(balanceOriginal.Value);

            var receipt = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(toAddress, 1.11m, gasPriceGwei: 2, new BigInteger(25000)).ConfigureAwait(false);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(balanceOriginalEther + 1.11m, Web3.Web3.Convert.FromWei(balance));
        }

        [Fact]
        public async void ShouldTransferEtherEstimatingAmount()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BA1";
            var balanceOriginal = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            var balanceOriginalEther = Web3.Web3.Convert.FromWei(balanceOriginal.Value);
            var transferService = web3.Eth.GetEtherTransferService();
            var estimate = await transferService.EstimateGasAsync(toAddress, 1.11m).ConfigureAwait(false);
            var receipt = await transferService
                .TransferEtherAndWaitForReceiptAsync(toAddress, 1.11m, gasPriceGwei: 2, estimate).ConfigureAwait(false);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(balanceOriginalEther + 1.11m, Web3.Web3.Convert.FromWei(balance));
        }

        [Fact]
        public async void ShouldTransferWholeBalanceInEther()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            var privateKey = EthECKey.GenerateKey();
            var newAccount = new Account(privateKey.GetPrivateKey(), EthereumClientIntegrationFixture.ChainId);
            var toAddress = newAccount.Address;
            var transferService = web3.Eth.GetEtherTransferService();

            var receipt = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(toAddress, 0.1m, gasPriceGwei: 2).ConfigureAwait(false);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(0.1m, Web3.Web3.Convert.FromWei(balance));

            var totalAmountBack =
                await transferService.CalculateTotalAmountToTransferWholeBalanceInEtherAsync(toAddress, 2m).ConfigureAwait(false);

            var web32 = new Web3.Web3(newAccount, web3.Client);
            var receiptBack = await web32.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(EthereumClientIntegrationFixture.AccountAddress, totalAmountBack, gasPriceGwei: 2).ConfigureAwait(false);

            var balanceFrom = await web32.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(0, Web3.Web3.Convert.FromWei(balanceFrom));
        }

        [Fact]
        public async void ShouldTransferWholeBalanceInEtherEIP1599()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            var privateKey = EthECKey.GenerateKey();
            var newAccount = new Account(privateKey.GetPrivateKey(), EthereumClientIntegrationFixture.ChainId);
            var toAddress = newAccount.Address;
            var transferService = web3.Eth.GetEtherTransferService();

            var fee = await transferService.SuggestFeeToTransferWholeBalanceInEtherAsync().ConfigureAwait(false);
            var receipt = await transferService
                .TransferEtherAndWaitForReceiptAsync(toAddress, 0.1m, maxPriorityFee: fee.MaxPriorityFeePerGas.Value, maxFeePerGas: fee.MaxFeePerGas.Value).ConfigureAwait(false);

            var balance = await web3.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(0.1m, Web3.Web3.Convert.FromWei(balance));


            var web32 = new Web3.Web3(newAccount, web3.Client);

            var feeWhole =
                  await web32.Eth.GetEtherTransferService().SuggestFeeToTransferWholeBalanceInEtherAsync().ConfigureAwait(false);
            
            var amount = await web32.Eth.GetEtherTransferService()
                .CalculateTotalAmountToTransferWholeBalanceInEtherAsync(toAddress, maxFeePerGas: feeWhole.MaxFeePerGas.Value).ConfigureAwait(false);
            
            var receiptBack = await web32.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(EthereumClientIntegrationFixture.AccountAddress, amount, feeWhole.MaxPriorityFeePerGas.Value, feeWhole.MaxFeePerGas.Value).ConfigureAwait(false);

            var balanceFrom = await web32.Eth.GetBalance.SendRequestAsync(toAddress).ConfigureAwait(false);
            Assert.Equal(0, Web3.Web3.Convert.FromWei(balanceFrom));
        }
    }
}

﻿using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Model;
using Nethereum.RPC.TransactionManagers;
using Nethereum.Signer;
using Nethereum.XUnitEthereumClients;
using Xunit;

namespace Nethereum.Accounts.IntegrationTests
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class TransactionRawRecovery
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public TransactionRawRecovery(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        [Fact]
        public async void ShouldRecoverRawTransactionFromRPCTransactionAndAccountSender()
        {

            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
           
            var transactionManager = web3.TransactionManager;
            var fromAddress = transactionManager?.Account?.Address;

            //Sending transaction
            var transactionInput = EtherTransferTransactionInputBuilder.CreateTransactionInput(fromAddress, toAddress, 1.11m, 2);
            //Raw transaction signed
            var rawTransaction = await transactionManager.SignTransactionAsync(transactionInput).ConfigureAwait(false);
            var txnHash = await web3.Eth.Transactions.SendRawTransaction.SendRequestAsync(rawTransaction).ConfigureAwait(false);
            //Getting the transaction from the chain
            var transactionRpc = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txnHash).ConfigureAwait(false);
            
            //Using the transanction from RPC to build a txn for signing / signed
            var transaction = TransactionFactory.CreateLegacyTransaction(transactionRpc.To, transactionRpc.Gas, transactionRpc.GasPrice, transactionRpc.Value, transactionRpc.Input, transactionRpc.Nonce,
                transactionRpc.R, transactionRpc.S, transactionRpc.V);
            
            //Get the raw signed rlp recovered
            var rawTransactionRecovered = transaction.GetRLPEncoded().ToHex();
            
            //Get the account sender recovered
            var accountSenderRecovered = string.Empty;
            if (transaction is LegacyTransactionChainId)
            {
                var txnChainId = transaction as LegacyTransactionChainId;
                accountSenderRecovered = EthECKey.RecoverFromSignature(transaction.Signature.ToEthECDSASignature(), transaction.RawHash, txnChainId.GetChainIdAsBigInteger()).GetPublicAddress();
            }
            else
            {
                accountSenderRecovered = EthECKey.RecoverFromSignature(transaction.Signature.ToEthECDSASignature(), transaction.RawHash).GetPublicAddress();
            }

            Assert.Equal(rawTransaction, rawTransactionRecovered);
            Assert.Equal(web3.TransactionManager.Account.Address, accountSenderRecovered);
        }
    }
}

﻿using System.Collections.Generic;
using Nethereum.Siwe.Core;
using Xunit;

namespace Nethereum.Siwe.UnitTests
{
    public class ParserBuilderTests
    {
        [Fact]
        public void ShouldParseAndBuildWith2OptionalFields()
        {
            var message = "service.org wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24.000Z\nResources:\n- ipfs://Qme7ss3ARVgxv6rXqVPiikMJ8u2NLgmgszg13pYrDKEoiu\n- https://example.com/my-web2-claim.json";
            var domain = "service.org";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24.000Z";
            var resource1 = "ipfs://Qme7ss3ARVgxv6rXqVPiikMJ8u2NLgmgszg13pYrDKEoiu";
            var resource2 = "https://example.com/my-web2-claim.json";


            var decodedMessage = SiweMessageParser.Parse(message);
            
            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);
            Assert.Equal(resource1, decodedMessage.Resources[0]);
            Assert.Equal(resource2, decodedMessage.Resources[1]);

            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);
            Assert.Equal(resource1, decodedMessage2.Resources[0]);
            Assert.Equal(resource2, decodedMessage2.Resources[1]);

            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }


        [Fact]
        public void ShouldParseAndBuildWithNoOptionalFields()
        {
            var message = "service.org wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24.000Z";
            var domain = "service.org";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24.000Z";


            var decodedMessage = SiweMessageParser.Parse(message);

            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);


            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);
 

            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }


        [Fact]
        public void ShouldParseAndBuildTimestampWithoutMicroseconds()
        {
            var message = "service.org wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24Z";
            var domain = "service.org";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24Z";


            var decodedMessage = SiweMessageParser.Parse(message);

            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);


            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);


            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }

        [Fact]
        public void ShouldParseAndBuildWithDomainRFC3986AndUserInfo()
        {
            var message = "test@127.0.0.1 wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24.000Z";
            var domain = "test@127.0.0.1";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24.000Z";


            var decodedMessage = SiweMessageParser.Parse(message);

            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);


            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);


            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }

        [Fact]
        public void ShouldParseAndBuildWithDomainRFC3986AndPort()
        {
            var message = "127.0.0.1:8080 wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24.000Z";
            var domain = "127.0.0.1:8080";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24.000Z";


            var decodedMessage = SiweMessageParser.Parse(message);

            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);


            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);


            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }

        [Fact]
        public void ShouldParseAndBuildWithDomainRFC3986WithUserInfoAndPort()
        {
            var message = "test@127.0.0.1:8080 wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24.000Z";
            var domain = "test@127.0.0.1:8080";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24.000Z";


            var decodedMessage = SiweMessageParser.Parse(message);

            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);


            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);


            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }

        [Fact]
        public void ShouldParseAndBuildWithAllOptionalFields()
        {
            var message = "test@127.0.0.1:8080 wants you to sign in with your Ethereum account:\n0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2\n\nI accept the ServiceOrg Terms of Service: https://service.org/tos\n\nURI: https://service.org/login\nVersion: 1\nChain ID: 1\nNonce: 32891757\nIssued At: 2021-09-30T16:25:24.000Z\nExpiration Time: 2021-09-30T16:25:24.000Z\nNot Before: 2021-09-30T16:25:24.000Z\nRequest ID: 200\nResources:\n- ipfs://Qme7ss3ARVgxv6rXqVPiikMJ8u2NLgmgszg13pYrDKEoiu\n- https://example.com/my-web2-claim.json";
            var domain = "test@127.0.0.1:8080";
            var address = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
            var statement = "I accept the ServiceOrg Terms of Service: https://service.org/tos";
            var uri = "https://service.org/login";
            var version = "1";
            var chainId = "1";
            var nonce = "32891757";
            var issuedAt = "2021-09-30T16:25:24.000Z";
            var expirationTime = "2021-09-30T16:25:24.000Z";
            var notBefore = "2021-09-30T16:25:24.000Z";
            var requestId = "200";
            var resource1 = "ipfs://Qme7ss3ARVgxv6rXqVPiikMJ8u2NLgmgszg13pYrDKEoiu";
            var resource2 = "https://example.com/my-web2-claim.json";

            var decodedMessage = SiweMessageParser.Parse(message);
            Assert.Equal(domain, decodedMessage.Domain);
            Assert.Equal(address, decodedMessage.Address);
            Assert.Equal(statement, decodedMessage.Statement);
            Assert.Equal(uri, decodedMessage.Uri);
            Assert.Equal(version, decodedMessage.Version);
            Assert.Equal(chainId, decodedMessage.ChainId);
            Assert.Equal(nonce, decodedMessage.Nonce);
            Assert.Equal(issuedAt, decodedMessage.IssuedAt);
            Assert.Equal(expirationTime, decodedMessage.ExpirationTime);
            Assert.Equal(notBefore, decodedMessage.NotBefore);
            Assert.Equal(requestId, decodedMessage.RequestId);

            Assert.Equal(resource1, decodedMessage.Resources[0]);
            Assert.Equal(resource2, decodedMessage.Resources[1]);

            var decodedMessage2 = SiweMessageParser.ParseUsingAbnf(message);

            Assert.Equal(domain, decodedMessage2.Domain);
            Assert.Equal(address, decodedMessage2.Address);
            Assert.Equal(statement, decodedMessage2.Statement);
            Assert.Equal(uri, decodedMessage2.Uri);
            Assert.Equal(version, decodedMessage2.Version);
            Assert.Equal(chainId, decodedMessage2.ChainId);
            Assert.Equal(nonce, decodedMessage2.Nonce);
            Assert.Equal(issuedAt, decodedMessage2.IssuedAt);
            Assert.Equal(expirationTime, decodedMessage2.ExpirationTime);
            Assert.Equal(notBefore, decodedMessage2.NotBefore);
            Assert.Equal(requestId, decodedMessage2.RequestId);
            Assert.Equal(resource1, decodedMessage2.Resources[0]);
            Assert.Equal(resource2, decodedMessage2.Resources[1]);


            var builtMessage = SiweMessageStringBuilder.BuildMessage(decodedMessage2);
            Assert.Equal(message, builtMessage);
        }

    }
}

﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer.Crypto;
using System;
using System.Text;
using Nethereum.Util;
using System.Diagnostics;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts.Standards.ERC20.ContractDefinition;
using BenchmarkDotNet.Configs;
using Nethereum.Model;

namespace Nethereum.Signer.Performance
{
    //dotnet run -c Release
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MessageSignerTest>(DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));
        }
    }

    /*
// * Summary *

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.22000
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=6.0.400-preview.22301.10
  [Host]     : .NET Core 6.0.5 (CoreCLR 6.0.522.21309, CoreFX 6.0.522.21309), X64 RyuJIT
  DefaultJob : .NET Core 6.0.5 (CoreCLR 6.0.522.21309, CoreFX 6.0.522.21309), X64 RyuJIT


|                    Method |          Mean |      Error |     StdDev |
|-------------------------- |--------------:|-----------:|-----------:|
|         MessageSigningRec |     0.1944 ns |  0.0028 ns |  0.0025 ns |
|      MessageSigningBouncy |     2.5965 ns |  0.0275 ns |  0.0257 ns |
|                  Recovery |     0.2177 ns |  0.0035 ns |  0.0033 ns |
|            RecoveryBouncy |     1.7579 ns |  0.0300 ns |  0.0281 ns |
|              FullRoundRec |     1.7343 ns |  0.0069 ns |  0.0061 ns |
|           FullRoundBouncy |     6.7162 ns |  0.1183 ns |  0.1408 ns |
|       SignFunctionMessage | 4,051.9825 ns | 16.8164 ns | 15.7300 ns |
| SignFunctionMessageBouncy | 4,051.6605 ns | 17.6386 ns | 15.6361 ns |

4,051ns == 0.004045 milliseconds
    */
    public class MessageSignerTest
    {

        EthECKey key;
        byte[] message;

        public MessageSignerTest()
        {
            key = new EthECKey("b5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7".HexToByteArray(), true);
            message = Nethereum.Util.Sha3Keccack.Current.CalculateHash(Encoding.UTF8.GetBytes("Hello"));
        }

        [Benchmark(OperationsPerInvoke = 1000000)]
        public EthECDSASignature MessageSigningRec()
        {
            EthECKey.SignRecoverable = true;
            return key.SignAndCalculateV(message);
        }





        [Benchmark(OperationsPerInvoke = 1000000)]
        public EthECDSASignature MessageSigningBouncy()
        {
            EthECKey.SignRecoverable = false;
            return key.SignAndCalculateV(message);
            
        }

        [Benchmark(OperationsPerInvoke = 1000000)]
        public string Recovery()
        {
            EthECKey.SignRecoverable = true;
            var signature =
                "0x0976a177078198a261faf206287b8bb93ebb233347ab09a57c8691733f5772f67f398084b30fc6379ffee2cc72d510fd0f8a7ac2ee0162b95dc5d61146b40ffa1c";
            var text = "test";
            var hasher = new Sha3Keccack();
            var hash = hasher.CalculateHash(text);
            var signer = new EthereumMessageSigner();
            var account = signer.EcRecover(hash.HexToByteArray(), signature);
            return account;
        }

        [Benchmark(OperationsPerInvoke = 1000000)]
        public string RecoveryBouncy()
        {
            EthECKey.SignRecoverable = false;
            var signature =
                "0x0976a177078198a261faf206287b8bb93ebb233347ab09a57c8691733f5772f67f398084b30fc6379ffee2cc72d510fd0f8a7ac2ee0162b95dc5d61146b40ffa1c";
            var text = "test";
            var hasher = new Sha3Keccack();
            var hash = hasher.CalculateHash(text);
            var signer = new EthereumMessageSigner();
            var account = signer.EcRecover(hash.HexToByteArray(), signature);
            return account;
        }

        [Benchmark(OperationsPerInvoke = 1000000)]
        public string FullRoundRec()
        {
            EthECKey.SignRecoverable = true;
            var key = EthECKey.GenerateKey();
            var address = key.GetPublicAddress();
            var signature = key.SignAndCalculateV(message);
            var recAdress = EthECKey.RecoverFromSignature(signature, message).GetPublicAddress();
            if (!address.IsTheSameAddress(recAdress))
            {
                Debug.WriteLine(key.GetPrivateKey());
            }
            return recAdress;
        }

        [Benchmark(OperationsPerInvoke = 1000000)]
        public string FullRoundBouncy()
        {
            EthECKey.SignRecoverable = false;
            var key = EthECKey.GenerateKey();
            var address = key.GetPublicAddress();
            var signature = key.SignAndCalculateV(message);
            var recAdress = EthECKey.RecoverFromSignature(signature, message).GetPublicAddress();
            if (!address.IsTheSameAddress(recAdress))
            {
                Debug.WriteLine(key.GetPrivateKey());
            }
            return recAdress;
        }



        [Benchmark(OperationsPerInvoke = 1000000)]
        public string SignFunctionMessage()
        {
            EthECKey.SignRecoverable = true;
            var web3 = new Web3.Web3(new Web3.Accounts.Account("0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7"));
            var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
            var fromAddress = web3.TransactionManager.Account.Address;
            var transactionMessage = new TransferFunction
            {
                FromAddress = fromAddress,
                To = newAddress,
                Value = 1000,
                MaxFeePerGas = 1000,
                MaxPriorityFeePerGas = 1000,
                Nonce = 0,
                TransactionType = TransactionType.EIP1559.AsByte(),
                Gas = 1000
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var signature =  transferHandler.SignTransactionAsync(fromAddress, transactionMessage).Result;
            return signature;
        }


        [Benchmark(OperationsPerInvoke = 1000000)]
        public string SignFunctionMessageBouncy()
        {
            EthECKey.SignRecoverable = false;
            var web3 = new Web3.Web3(new Web3.Accounts.Account("0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7"));
            var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
            var fromAddress = web3.TransactionManager.Account.Address;
            var transactionMessage = new TransferFunction
            {
                FromAddress = fromAddress,
                To = newAddress,
                Value = 1000,
                MaxFeePerGas = 1000,
                MaxPriorityFeePerGas = 1000,
                Nonce = 0,
                TransactionType = TransactionType.EIP1559.AsByte(),
                Gas = 1000
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var signature = transferHandler.SignTransactionAsync(fromAddress, transactionMessage).Result;
            return signature;
        }
    }

}


﻿using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.XUnitEthereumClients;
using Xunit; 
 // ReSharper disable ConsiderUsingConfigureAwait  
 // ReSharper disable AsyncConverter.ConfigureAwaitHighlighting 
 // ReSharper disable ConsiderUsingConfigureAwait  
 // ReSharper disable AsyncConverter.ConfigureAwaitHighlighting 



namespace Nethereum.Contracts.IntegrationTests.CQS
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class ContractHandlers
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public ContractHandlers(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        [Fact]
        public async void ShouldDecodeTransactionDeployment()
        {
            //EthereumClientIntegrationFixture.AccountAddress
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 10000,
                FromAddress = senderAddress,
                Gas = new HexBigInteger(900000)
            };

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage).ConfigureAwait(false);

            var transaction =
                await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionReceipt.TransactionHash).ConfigureAwait(false);

            var deploymentMessageDecoded = transaction.DecodeTransactionToDeploymentMessage<StandardTokenDeployment>();

            Assert.Equal(deploymentMessage.TotalSupply, deploymentMessageDecoded.TotalSupply);
            Assert.Equal(deploymentMessage.FromAddress.ToLower(), deploymentMessageDecoded.FromAddress.ToLower());
            Assert.Equal(deploymentMessage.Gas, deploymentMessageDecoded.Gas);
        }


        [Fact]
        public async void ShouldDecodeTransactionInput()
        {
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 10000,
                FromAddress = senderAddress,
                Gas = new HexBigInteger(900000)
            };

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

            var contractAddress = transactionReceipt.ContractAddress;
            var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";


            var transactionMessage = new TransferFunction
            {
                FromAddress = senderAddress,
                To = newAddress,
                TokenAmount = 1000
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transferReceipt =
                await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transactionMessage);

            var transaction =
                await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transferReceipt.TransactionHash);

            var transferDecoded = transaction.DecodeTransactionToFunctionMessage<TransferFunction>();

            Assert.Equal(transactionMessage.To.ToLower(), transferDecoded.To.ToLower());
            Assert.Equal(transactionMessage.FromAddress.ToLower(), transferDecoded.FromAddress.ToLower());
            Assert.Equal(transactionMessage.TokenAmount, transferDecoded.TokenAmount);
        }

        [Fact]
        public async void Test()
        {
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 10000,
                FromAddress = senderAddress,
                Gas = new HexBigInteger(900000)
            };

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

            var contractAddress = transactionReceipt.ContractAddress;
            var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";


            var transactionMessage = new TransferFunction
            {
                FromAddress = senderAddress,
                To = newAddress,
                TokenAmount = 1000
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

            var estimatedGas = await transferHandler.EstimateGasAsync(contractAddress, transactionMessage);

            // for demo purpouses gas estimation it is done in the background so we don't set it
            transactionMessage.Gas = estimatedGas.Value;

            var transferReceipt =
                await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transactionMessage);

            var balanceOfFunctionMessage = new BalanceOfFunction
            {
                Owner = newAddress,
                FromAddress = senderAddress
            };

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var balanceFirstTransaction =
                await balanceHandler.QueryAsync<int>(contractAddress, balanceOfFunctionMessage);


            Assert.Equal(1000, balanceFirstTransaction);

            var transferReceipt2 =
                await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transactionMessage);
            var balanceSecondTransaction =
                await balanceHandler.QueryDeserializingToObjectAsync<BalanceOfFunctionOutput>(balanceOfFunctionMessage,
                    contractAddress);

            Assert.Equal(2000, balanceSecondTransaction.Balance);

            var balanceFirstTransactionHistory =
                await balanceHandler.QueryDeserializingToObjectAsync<BalanceOfFunctionOutput>(balanceOfFunctionMessage,
                    contractAddress, new BlockParameter(transferReceipt.BlockNumber));

            Assert.Equal(1000, balanceFirstTransactionHistory.Balance);
        }
    }

    [Function("transfer", "bool")]
    public class TransferFunction : FunctionMessage
    {
        [Parameter("address", "_to", 1)] public string To { get; set; }

        [Parameter("uint256", "_value", 2)] public int TokenAmount { get; set; }
    }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)] public string Owner { get; set; }
    }

    [FunctionOutput]
    public class BalanceOfFunctionOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)] public int Balance { get; set; }
    }

    public class StandardTokenDeployment : ContractDeploymentMessage
    {
        public static string BYTECODE =
            "0x60606040526040516020806106f5833981016040528080519060200190919050505b80600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005081905550806000600050819055505b506106868061006f6000396000f360606040523615610074576000357c010000000000000000000000000000000000000000000000000000000090048063095ea7b31461008157806318160ddd146100b657806323b872dd146100d957806370a0823114610117578063a9059cbb14610143578063dd62ed3e1461017857610074565b61007f5b610002565b565b005b6100a060048080359060200190919080359060200190919050506101ad565b6040518082815260200191505060405180910390f35b6100c36004805050610674565b6040518082815260200191505060405180910390f35b6101016004808035906020019091908035906020019091908035906020019091905050610281565b6040518082815260200191505060405180910390f35b61012d600480803590602001909190505061048d565b6040518082815260200191505060405180910390f35b61016260048080359060200190919080359060200190919050506104cb565b6040518082815260200191505060405180910390f35b610197600480803590602001909190803590602001909190505061060b565b6040518082815260200191505060405180910390f35b600081600260005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008573ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925846040518082815260200191505060405180910390a36001905061027b565b92915050565b600081600160005060008673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561031b575081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505410155b80156103275750600082115b1561047c5781600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a381600160005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505403925050819055506001905061048656610485565b60009050610486565b5b9392505050565b6000600160005060008373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505490506104c6565b919050565b600081600160005060003373ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561050c5750600082115b156105fb5781600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a36001905061060556610604565b60009050610605565b5b92915050565b6000600260005060008473ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005054905061066e565b92915050565b60006000600050549050610683565b9056";

        public StandardTokenDeployment() : base(BYTECODE)
        {
        }

        [Parameter("uint256", "totalSupply")] public int TotalSupply { get; set; }
    }

    [Event("Transfer")]
    public class TransferEventDTOBase : IEventDTO
    {
        [Parameter("address", "_from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "_to", 2, true)] public string To { get; set; }

        [Parameter("uint256", "_value", 3, false)]
        public BigInteger Value { get; set; }
    }

    public partial class TransferEventDTO : TransferEventDTOBase
    {
    }
}

﻿using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.XUnitEthereumClients;
using Xunit; 
 // ReSharper disable ConsiderUsingConfigureAwait  
 // ReSharper disable AsyncConverter.ConfigureAwaitHighlighting 
 // ReSharper disable ConsiderUsingConfigureAwait  
 // ReSharper disable AsyncConverter.ConfigureAwaitHighlighting 



namespace Nethereum.Contracts.IntegrationTests.CQS
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class ContractHandlers
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public ContractHandlers(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        [Fact]
        public async void ShouldDecodeTransactionDeployment()
        {
            //EthereumClientIntegrationFixture.AccountAddress
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 10000,
                FromAddress = senderAddress,
                Gas = new HexBigInteger(900000)
            };

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage).ConfigureAwait(false);

            var transaction =
                await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionReceipt.TransactionHash).ConfigureAwait(false);

            var deploymentMessageDecoded = transaction.DecodeTransactionToDeploymentMessage<StandardTokenDeployment>();

            Assert.Equal(deploymentMessage.TotalSupply, deploymentMessageDecoded.TotalSupply);
            Assert.Equal(deploymentMessage.FromAddress.ToLower(), deploymentMessageDecoded.FromAddress.ToLower());
            Assert.Equal(deploymentMessage.Gas, deploymentMessageDecoded.Gas);
        }


        [Fact]
        public async void ShouldDecodeTransactionInput()
        {
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 10000,
                FromAddress = senderAddress,
                Gas = new HexBigInteger(900000)
            };

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

            var contractAddress = transactionReceipt.ContractAddress;
            var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";


            var transactionMessage = new TransferFunction
            {
                FromAddress = senderAddress,
                To = newAddress,
                TokenAmount = 1000
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transferReceipt =
                await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transactionMessage);

            var transaction =
                await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transferReceipt.TransactionHash);

            var transferDecoded = transaction.DecodeTransactionToFunctionMessage<TransferFunction>();

            Assert.Equal(transactionMessage.To.ToLower(), transferDecoded.To.ToLower());
            Assert.Equal(transactionMessage.FromAddress.ToLower(), transferDecoded.FromAddress.ToLower());
            Assert.Equal(transactionMessage.TokenAmount, transferDecoded.TokenAmount);
        }

        [Fact]
        public async void Test()
        {
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var deploymentMessage = new StandardTokenDeployment
            {
                TotalSupply = 10000,
                FromAddress = senderAddress,
                Gas = new HexBigInteger(900000)
            };

            var deploymentHandler = web3.Eth.GetContractDeploymentHandler<StandardTokenDeployment>();
            var transactionReceipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);

            var contractAddress = transactionReceipt.ContractAddress;
            var newAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";


            var transactionMessage = new TransferFunction
            {
                FromAddress = senderAddress,
                To = newAddress,
                TokenAmount = 1000
            };

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();

            var estimatedGas = await transferHandler.EstimateGasAsync(contractAddress, transactionMessage);

            // for demo purpouses gas estimation it is done in the background so we don't set it
            transactionMessage.Gas = estimatedGas.Value;

            var transferReceipt =
                await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transactionMessage);

            var balanceOfFunctionMessage = new BalanceOfFunction
            {
                Owner = newAddress,
                FromAddress = senderAddress
            };

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var balanceFirstTransaction =
                await balanceHandler.QueryAsync<int>(contractAddress, balanceOfFunctionMessage);


            Assert.Equal(1000, balanceFirstTransaction);

            var transferReceipt2 =
                await transferHandler.SendRequestAndWaitForReceiptAsync(contractAddress, transactionMessage);
            var balanceSecondTransaction =
                await balanceHandler.QueryDeserializingToObjectAsync<BalanceOfFunctionOutput>(balanceOfFunctionMessage,
                    contractAddress);

            Assert.Equal(2000, balanceSecondTransaction.Balance);

            var balanceFirstTransactionHistory =
                await balanceHandler.QueryDeserializingToObjectAsync<BalanceOfFunctionOutput>(balanceOfFunctionMessage,
                    contractAddress, new BlockParameter(transferReceipt.BlockNumber));

            Assert.Equal(1000, balanceFirstTransactionHistory.Balance);
        }
    }

    [Function("transfer", "bool")]
    public class TransferFunction : FunctionMessage
    {
        [Parameter("address", "_to", 1)] public string To { get; set; }

        [Parameter("uint256", "_value", 2)] public int TokenAmount { get; set; }
    }

    [Function("balanceOf", "uint256")]
    public class BalanceOfFunction : FunctionMessage
    {
        [Parameter("address", "_owner", 1)] public string Owner { get; set; }
    }

    [FunctionOutput]
    public class BalanceOfFunctionOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", 1)] public int Balance { get; set; }
    }

    public class StandardTokenDeployment : ContractDeploymentMessage
    {
        public static string BYTECODE =
            "0x60606040526040516020806106f5833981016040528080519060200190919050505b80600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005081905550806000600050819055505b506106868061006f6000396000f360606040523615610074576000357c010000000000000000000000000000000000000000000000000000000090048063095ea7b31461008157806318160ddd146100b657806323b872dd146100d957806370a0823114610117578063a9059cbb14610143578063dd62ed3e1461017857610074565b61007f5b610002565b565b005b6100a060048080359060200190919080359060200190919050506101ad565b6040518082815260200191505060405180910390f35b6100c36004805050610674565b6040518082815260200191505060405180910390f35b6101016004808035906020019091908035906020019091908035906020019091905050610281565b6040518082815260200191505060405180910390f35b61012d600480803590602001909190505061048d565b6040518082815260200191505060405180910390f35b61016260048080359060200190919080359060200190919050506104cb565b6040518082815260200191505060405180910390f35b610197600480803590602001909190803590602001909190505061060b565b6040518082815260200191505060405180910390f35b600081600260005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008573ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925846040518082815260200191505060405180910390a36001905061027b565b92915050565b600081600160005060008673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561031b575081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505410155b80156103275750600082115b1561047c5781600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a381600160005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600260005060008673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060003373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505403925050819055506001905061048656610485565b60009050610486565b5b9392505050565b6000600160005060008373ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000505490506104c6565b919050565b600081600160005060003373ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600050541015801561050c5750600082115b156105fb5781600160005060003373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282825054039250508190555081600160005060008573ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828282505401925050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a36001905061060556610604565b60009050610605565b5b92915050565b6000600260005060008473ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005060008373ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060005054905061066e565b92915050565b60006000600050549050610683565b9056";

        public StandardTokenDeployment() : base(BYTECODE)
        {
        }

        [Parameter("uint256", "totalSupply")] public int TotalSupply { get; set; }
    }

    [Event("Transfer")]
    public class TransferEventDTOBase : IEventDTO
    {
        [Parameter("address", "_from", 1, true)]
        public string From { get; set; }

        [Parameter("address", "_to", 2, true)] public string To { get; set; }

        [Parameter("uint256", "_value", 3, false)]
        public BigInteger Value { get; set; }
    }

    public partial class TransferEventDTO : TransferEventDTOBase
    {
    }
}

﻿using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Chain;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionTypes;
using Nethereum.XUnitEthereumClients;
using Xunit;

namespace Nethereum.Accounts.IntegrationTests
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class TransactionTests
    {

        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public TransactionTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        [Fact]
        public async void ShouldReceiveTheTransactionHash()
        {

            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
            var receipt = await web3.Eth.GetEtherTransferService()
                .TransferEtherAndWaitForReceiptAsync(toAddress, 1.11m, gasPriceGwei: 2).ConfigureAwait(false);
            var tran = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(receipt.TransactionHash);
            Assert.NotNull(tran.TransactionHash);
            var blockWithTransactions =
                await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(receipt.BlockNumber);
            foreach (var transaction in blockWithTransactions.Transactions)
            {
                Assert.NotNull(transaction.TransactionHash);
            }
        }

        [Fact]
        public async void ShouldReceiveTheTransactionByHashPendingAndNullValuesDependingOnTrasactionType()
        {

            var web3 = _ethereumClientIntegrationFixture.GetWeb3();

            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
            var transactionHash = await web3.Eth.GetEtherTransferService()
                .TransferEtherAsync(toAddress, 1.11m, gasPriceGwei: 2).ConfigureAwait(false);
            var tran = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            Assert.NotNull(tran.TransactionHash);
            Assert.Null(tran.MaxFeePerGas);
            Assert.Null(tran.MaxPriorityFeePerGas);
            Assert.NotNull(tran.GasPrice);

            var transactionHash2 = await web3.Eth.GetEtherTransferService()
                .TransferEtherAsync(toAddress, 1.11m, maxFeePerGas: Web3.Web3.Convert.ToWei(2, Util.UnitConversion.EthUnit.Gwei), maxPriorityFee: Web3.Web3.Convert.ToWei(2, Util.UnitConversion.EthUnit.Gwei)).ConfigureAwait(false);
            var tran2 = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash2);
            Assert.NotNull(tran2.TransactionHash);
            Assert.NotNull(tran2.MaxFeePerGas);
            Assert.NotNull(tran2.MaxPriorityFeePerGas);
            Assert.NotNull(tran2.GasPrice);

        }


        [Fact]
        public async void ShouldSendTrasactionBasedOnChainFeature()
        {

            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            ChainFeaturesService.Current.UpsertChainFeature(
                new ChainFeature()
                {
                    ChainName = "Nethereum Test Chain",
                    ChainId = 444444444500,
                    SupportEIP1559 = false
                });

            var toAddress = "0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe";
            var tranHash = await web3.Eth.TransactionManager.SendTransactionAsync(new TransactionInput()
            {
                From = EthereumClientIntegrationFixture.AccountAddress,
                To = toAddress,
                Value = new HexBigInteger(100),
            }
            );
                
            var tran = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(tranHash);
            Assert.NotNull(tran.TransactionHash);
            Assert.Null(tran.MaxFeePerGas);
            Assert.Null(tran.MaxPriorityFeePerGas);
            Assert.NotNull(tran.GasPrice);

            ChainFeaturesService.Current.UpsertChainFeature(
                new ChainFeature()
                {
                    ChainName = "Nethereum Test Chain",
                    ChainId = 444444444500,
                    SupportEIP1559 = true
                });

            var tranHash2 = await web3.Eth.TransactionManager.SendTransactionAsync(new TransactionInput()
            {
                From = EthereumClientIntegrationFixture.AccountAddress,
                To = toAddress,
                Value = new HexBigInteger(100),
            }
            );

            var tran2 = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(tranHash2);
            Assert.NotNull(tran2.TransactionHash);
            Assert.NotNull(tran2.MaxFeePerGas);
            Assert.NotNull(tran2.MaxPriorityFeePerGas);
            Assert.NotNull(tran2.GasPrice);

            ChainFeaturesService.Current.TryRemoveChainFeature(444444444500);
            //Should default to 1559 when not feature is set

            var tranHash3 = await web3.Eth.TransactionManager.SendTransactionAsync(new TransactionInput()
            {
                From = EthereumClientIntegrationFixture.AccountAddress,
                To = toAddress,
                Value = new HexBigInteger(100),
            }
           );

            var tran3 = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(tranHash3);
            Assert.NotNull(tran3.TransactionHash);
            Assert.NotNull(tran3.MaxFeePerGas);
            Assert.NotNull(tran3.MaxPriorityFeePerGas);
            Assert.NotNull(tran3.GasPrice);
        }

        [Fact]
        public async void ShouldGetTransactionByHash()
        {
            var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Mainnet);
            var txnType2 = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync("0xe7bab1a12b9234a27a0f53f71d19bc0595f1ea2c8148f5d45edac76a4566e15b");
            var txnLegacy = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync("0x8751032c189f44478b13ca77834b6af3567ec3e014069450f17209ed0fd1a3c1");
            Assert.True(txnType2.Type.ToTransactionType() == TransactionType.EIP1559);
            Assert.True(txnLegacy.Type.ToTransactionType() == TransactionType.Legacy);

            Assert.True(txnType2.Is1559());
            Assert.True(txnLegacy.IsLegacy());
        }

    }
}

﻿using Nethereum.ABI;
using Nethereum.ABI.Decoders;
using Nethereum.ABI.Encoders;
using Nethereum.EVM;
using Nethereum.EVM.BlockchainState;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.XUnitEthereumClients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Nethereum.RPC.DebugNode;
using Nethereum.Util;
// ReSharper disable ConsiderUsingConfigureAwait  
// ReSharper disable AsyncConverter.ConfigureAwaitHighlighting

namespace Nethereum.Contracts.IntegrationTests.EVM
{
    public class ExternalTrace
    {
        [JsonProperty("pc")]
        public int Pc { get; set; }

        [JsonProperty("op")]
        public string Op { get; set; }

        [JsonProperty("gas")]
        public int Gas { get; set; }

        [JsonProperty("gasCost")]
        public int GasCost { get; set; }

        [JsonProperty("depth")]
        public int Depth { get; set; }

        [JsonProperty("stack")]
        public List<string> Stack { get; set; }

        [JsonProperty("memory")]
        public List<string> Memory { get; set; }
    }

    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class EvmChainTransactionLogsAndTracesTests
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public EvmChainTransactionLogsAndTracesTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }

        [Fact]
        public async void ShouldRetrieveUniswapV2TransactionFromChainAndSimulateIt()
        {
            //Uniswap
            await ShouldRetrieveTransactionFromChainSimulateItAndValidateLogs("0xb9f4e6e5c90329a43da70ced8e8974c3fa34e67e32283bfa82778296fa79dd98");

        }

        [Fact]
        public async void ShouldRetrieveOpenSeaTransferHelperTransactionFromChainAndSimulateIt()
        {
            //Open transfer helper
            await ShouldRetrieveTransactionFromChainSimulateItAndValidateLogs("0x2ab5b72b40d8d004d40258e7a8296d512a0d805c1f73603ddba4069a80e40946");

        }

        [Fact]
        public async void ShouldRetrieveCurveRemoveLiquidiyTransactionFromChainAndSimulateIt()
        {
            //Curve remove liquidity
            await ShouldRetrieveTransactionFromChainSimulateItAndValidateLogs("0x763774a4a954d0deccf9d054ed8164cef1e6762a45cdc30457b5c2770c833300");

        }

        [Fact]
        public async void ShouldRetrieveUniswapV3TransactionFromChainAndSimulateIt()
        {
            //Uniswap v3 multicall
            await ShouldRetrieveTransactionFromChainSimulateItAndValidateLogs("0x6669284f4072af03600f95bc4c1ed3499e1658dab87615cfd03775fea13a82b7", ConfigureStateUniswapV3);
        }
        //

        [Fact]
        public async void ShouldRetrieveUniswapV2TransactionFromChainAndValidateTraces()
        {
            //Uniswap // Gas "7f281"
            await RetrieveTransactionFromChainAndCompareToExternalTraces("0xb9f4e6e5c90329a43da70ced8e8974c3fa34e67e32283bfa82778296fa79dd98", "EVM/Traces/0xb9f4e6e5c90329a43da70ced8e8974c3fa34e67e32283bfa82778296fa79dd98.json", "7f281");

        }

        [Fact]
        public async void ShouldRetrieveUniswapV3TransactionFromChainAndValidateTraces()
        {
            //Uniswap v3 multicall
            await RetrieveTransactionFromChainAndCompareToExternalTraces("0x6669284f4072af03600f95bc4c1ed3499e1658dab87615cfd03775fea13a82b7", "EVM/Traces/0x6669284f4072af03600f95bc4c1ed3499e1658dab87615cfd03775fea13a82b7.json", "54532", ConfigureStateUniswapV3);

        }

      

        public void ConfigureStateUniswapV3(ExecutionStateService executionStateService)
        {
            var x = executionStateService.CreateOrGetAccountExecutionState("0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48");
            x.UpsertStorageValue(BigInteger.Parse("8221335686466422652986625159663000664425034433962318634917087004497611930108"), "0x0000000000000000000000000000000000000000000000000000000000000001".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0x88e6A0c2dDD26FEEb64F039a2c41296FcB3f5640");
            x.UpsertStorageValue(BigInteger.Parse("0"), "00010002d002d0021b032561000000000000751e61ce7e5521bd06be0572e804".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0x88e6A0c2dDD26FEEb64F039a2c41296FcB3f5640");
            x.UpsertStorageValue(BigInteger.Parse("1"), "00000000000000000000000000000000000069e9ce583c34c9378d2bfbdca7c2".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0x88e6A0c2dDD26FEEb64F039a2c41296FcB3f5640");
            x.UpsertStorageValue(BigInteger.Parse("547"), "010000000000000001e969dfa10e758077407af4270008d2d9511b47637cccc7".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0x88e6A0c2dDD26FEEb64F039a2c41296FcB3f5640");
            x.UpsertStorageValue(BigInteger.Parse("548"), "010000000000000001e9695b4b6ce4c3f8acf8aacd0008d2073d1693637c8a13".HexToByteArray());


            x = executionStateService.CreateOrGetAccountExecutionState("0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2");
            x.UpsertStorageValue("390f6178407c9b8e95802b8659e6df8e34c1e3d4f8d6a49e6132bbcdd937b63a".HexToBigInteger(false), "0000000000000000000000000000000000000000000012badbfadd11621bee72".HexToByteArray());


            x = executionStateService.CreateOrGetAccountExecutionState("0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48");
            x.UpsertStorageValue("1f21a62c4538bacf2aabeca410f0fe63151869f172e03c0e00357ba26a341eff".HexToBigInteger(false), "00000000000000000000000000000000000000000000000000001a59f6af31ce".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0xA0b86991c6218b36c1d19D4a2e9Eb0cE3606eB48");
            x.UpsertStorageValue("5677c3185d8c751c0e35cf53d6ef0339fe159883cdb7332be8c08b4bb14d8639".HexToBigInteger(false), "000000000000000000000000000000000000000000000000000000030a5d5601".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0xa6Cc3C2531FdaA6Ae1A3CA84c2855806728693e8");
            x.UpsertStorageValue("0".HexToBigInteger(false), "00010000b400b40088ff35910000000000000000132f8182f124602fc6a05e3c".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0xa6Cc3C2531FdaA6Ae1A3CA84c2855806728693e8");
            x.UpsertStorageValue("0000000000000000000000000000000000000000000000000000000000000002".HexToBigInteger(false), "0000000000000000000000000000000002896a12b23323df7ebf261fc3162a37".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0x514910771AF9Ca656af840dff83E8264EcF986CA");
            x.UpsertStorageValue("9422ae262bd5bbe8254768a185116d59ff7f53e5e813d9c0ea3840cf28c230a0".HexToBigInteger(false), "00000000000000000000000000000000000000000000efbdc3fc3ec569174298".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0x514910771AF9Ca656af840dff83E8264EcF986CA");
            x.UpsertStorageValue("a577b5d40dbe170a6ef57d4bc918b3a35dc863863e8ba8f47b22280c3bb0e1b0".HexToBigInteger(false), "0000000000000000000000000000000000000000000000000000000000000001".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0xa6Cc3C2531FdaA6Ae1A3CA84c2855806728693e8");
            x.UpsertStorageValue("0000000000000000000000000000000000000000000000000000000000000090".HexToBigInteger(false), "01000049a500000000000020cd5beea6872254ac05fffdb885edd196637cccd3".HexToByteArray());

            x = executionStateService.CreateOrGetAccountExecutionState("0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2");
            x.UpsertStorageValue("2152d1f752d5b88a3178a813eda1508fbde034f11b826cc92dea66732e3d19a1".HexToBigInteger(false), "0000000000000000000000000000000000000000000001fa8af73951ebec5044".HexToByteArray());

        }

        [Fact]
        public async void ShouldRetrieveOpenSeaTransferHelperTransactionFromChainAndValidateTraces()
        {
            //Open transfer helper
            await RetrieveTransactionFromChainAndCompareToExternalTraces("0x2ab5b72b40d8d004d40258e7a8296d512a0d805c1f73603ddba4069a80e40946", "EVM/Traces/0x2ab5b72b40d8d004d40258e7a8296d512a0d805c1f73603ddba4069a80e40946.json", "19e88");

        }

        [Fact]
        public async void ShouldRetrieveCurveRemoveLiquidiyTransactionFromChainAndValidateTraces()
        {
            //Curve remove liquidity
            //Gas "53cc5"
            await RetrieveTransactionFromChainAndCompareToExternalTraces("0x763774a4a954d0deccf9d054ed8164cef1e6762a45cdc30457b5c2770c833300", "EVM/Traces/0x763774a4a954d0deccf9d054ed8164cef1e6762a45cdc30457b5c2770c833300.zip", "53cc5");

        }

        [Fact]
        public async Task TestRevert()
        {
            var transactionHash = "0xf3d2a323110370a4dc72c04c738bf9b45d14b03603ed70372128a3966c54fca6";

            var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Mainnet);
            var txn = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var txnReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(txn.BlockNumber);
            var code = await web3.Eth.GetCode.SendRequestAsync(txn.To); // runtime code;
            Program program = await ExecuteProgramAsync(web3, txn, block, code, null);
            Assert.NotNull(program.ProgramResult.GetRevertMessage());
        }

        public async Task ShouldRetrieveTransactionFromChainSimulateItAndValidateLogs(string transactionHash, Action<ExecutionStateService> configureState = null)
        {
            //Scenario of complex uniswap clone to run end to end a previous transaction and see logs etc
            var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Mainnet);
            var txn = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var txnReceipt = await web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(txn.BlockNumber);
            var code = await web3.Eth.GetCode.SendRequestAsync(txn.To); // runtime code;
            Program program = await ExecuteProgramAsync(web3, txn, block, code, configureState);

            Assert.Equal(txnReceipt.Failed(), program.ProgramResult.IsRevert);
            if (!txnReceipt.Failed())
            {
                Assert.True(program.ProgramResult.Logs.Count == txnReceipt.Logs.Count);
                if (program.ProgramResult.Logs.Count > 0)
                {
                    var receiptLogs = txnReceipt.Logs.ConvertToFilterLog();

                    for (int i = 0; i < program.ProgramResult.Logs.Count; i++)
                    {
                        var simulatorLog = program.ProgramResult.Logs[i];
                        var receiptLog = receiptLogs[i];
                        Assert.True(simulatorLog.Address.IsTheSameHex(receiptLog.Address));
                        Assert.True(simulatorLog.Data.IsTheSameHex(receiptLog.Data));
                        Assert.True(simulatorLog.Topics.Length == receiptLog.Topics.Length);
                        for (int x = 0; x < simulatorLog.Topics.Length; x++)
                        {
                            simulatorLog.Topics[x].ToString().IsTheSameHex(receiptLog.Topics[x].ToString());
                        }
                    }
                }
            }

        }

        public async Task RetrieveTransactionFromChainAndCompareToExternalTraces(string transactionHash, string externalTracePath, string gasValue, Action<ExecutionStateService> configureState = null, bool useDebugStorageAt = false)
        {
            var json = "";
            if(Path.GetExtension(externalTracePath) == ".zip")
            {
                json = Unzip(externalTracePath);
            }
            else
            {
               json = System.IO.File.ReadAllText(externalTracePath);
            }
            
            var externalTraces = JsonConvert.DeserializeObject<List<ExternalTrace>>(json);

            var web3 = _ethereumClientIntegrationFixture.GetInfuraWeb3(InfuraNetwork.Mainnet);
            var txn = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            var block = await web3.Eth.Blocks.GetBlockWithTransactionsHashesByNumber.SendRequestAsync(txn.BlockNumber);
            var code = await web3.Eth.GetCode.SendRequestAsync(txn.To); // runtime code;

            Program program = await ExecuteProgramAsync(web3, txn, block, code, configureState, useDebugStorageAt);
            if (program.ProgramResult.Exception != null)
            {
                Debug.WriteLine("Program failure, validating traces");
                Debug.WriteLine(program.ProgramResult.Exception.ToString());
            }

            var trace = program.Trace;
            for (int i = 0; i < trace.Count; i++)
            {

                Debug.WriteLine("Validating test step");
                Debug.WriteLine(trace[i].VMTraceStep);
                Debug.WriteLine(trace[i].Instruction.Instruction.ToString());
                Debug.WriteLine(trace[i].Instruction.Value.ToString());
                var traceStep = trace[i];
                var externalTrace = externalTraces[i];

                Assert.Equal(traceStep.Depth, externalTrace.Depth - 1);
                var instructionName = traceStep.Instruction.Instruction.ToString();
                if (instructionName == "KECCAK256" && externalTrace.Op == "SHA3") instructionName = "SHA3";
                Assert.Equal(instructionName, externalTrace.Op);
                Assert.Equal(traceStep.Stack.Count, externalTrace.Stack.Count);

                var reverseStack = externalTrace.Stack.ToArray().Reverse().ToArray();

                for (int x = 0; x < reverseStack.Length; x++)
                {
                    var stackElementTrace = traceStep.Stack[x];
                    var stackElementTraceTest = reverseStack[x];

                    if (string.IsNullOrEmpty(gasValue) || stackElementTrace.ToHexCompact() != gasValue) /// default gas lets ignore it GAS gets called and added to the stack
                    {

                        Assert.Equal(stackElementTrace.ToHexCompact(),
                            stackElementTraceTest.ToHexCompact());
                    }
                }

                var indexOfEmptyMemory = externalTrace.Memory.Count;
                if (string.IsNullOrEmpty(traceStep.Memory))
                {
                    indexOfEmptyMemory = 0;
                }
                else
                {
                    if (externalTrace.Memory.Count > traceStep.MemoryAsArray.Count)
                    {
                        indexOfEmptyMemory = traceStep.MemoryAsArray.Count;
                    }
                }


                for (int x = 0; x < traceStep.MemoryAsArray.Count; x++)
                {
                    var stackElementTrace = traceStep.MemoryAsArray[x];

                    if (stackElementTrace.Length != 64)
                    {
                        stackElementTrace = stackElementTrace.PadRight(64, '0');
                    }
                    var stackElementTraceTest = externalTrace.Memory[x];
                    Assert.Equal(stackElementTrace.ToHexCompact(),
                        stackElementTraceTest.ToHexCompact());
                }

                for (int x = indexOfEmptyMemory; x < externalTrace.Memory.Count; x++)
                {
                    var stackElementTraceTest = externalTrace.Memory[x];
                    Assert.True(string.IsNullOrEmpty(stackElementTraceTest.ToHexCompact()));
                }
            }



        }

        public static async Task<Program> ExecuteProgramAsync(Web3.Web3 web3, Transaction txn, BlockWithTransactionHashes block, string code, Action<ExecutionStateService> configureState = null, bool useDebugStorageAt = false)
        {
           
                //var instructions = ProgramInstructionsUtils.GetProgramInstructions(code);
                var txnInput = txn.ConvertToTransactionInput();
                txnInput.ChainId = new HexBigInteger(1);

                var nodeDataService = new RpcNodeDataService(web3.Eth, new BlockParameter(new HexBigInteger(txn.BlockNumber.Value - 1)));
                if (useDebugStorageAt)
                {
                    throw new Exception("Need an archive node configuration");
                    //var web32 = new Web3.Web3("https://rpc.archivenode.io/);
                    //nodeDataService = new RpcNodeDataService(web3.Eth, new BlockParameter(new HexBigInteger(txn.BlockNumber.Value - 1)), web32.Debug, block.BlockHash, (int)txn.TransactionIndex.Value));
                }


                var executionStateService = new ExecutionStateService(nodeDataService);
                if (configureState != null)
                {
                    configureState(executionStateService);
                }

                var programContext = new ProgramContext(txnInput, executionStateService, null, null, (long)txn.BlockNumber.Value, (long)block.Timestamp.Value);
                var program = new Program(code.HexToByteArray(), programContext);
                var evmSimulator = new EVMSimulator();

                try
                {
                    program = await evmSimulator.ExecuteAsync(program, 0, 0, true);
                    return program;

                }
                catch (Exception ex)
                {
                    program.ProgramResult.Exception = ex;
                    return program;
                }
          
        }

        public static string Unzip(string filePath)
        {
            var rawFileStream = File.OpenRead(filePath);
            byte[] zippedtoTextBuffer = new byte[rawFileStream.Length];
            rawFileStream.Read(zippedtoTextBuffer, 0, (int)rawFileStream.Length);

            return Unzip(zippedtoTextBuffer);
        }

     
        public static string Unzip(byte[] zippedBuffer)
        {
            using (var zippedStream = new MemoryStream(zippedBuffer))
            {
                using (var archive = new ZipArchive(zippedStream))
                {
                    var entry = archive.Entries.FirstOrDefault();

                    if (entry != null)
                    {
                        using (var unzippedEntryStream = entry.Open())
                        {
                            using (var ms = new MemoryStream())
                            {
                                unzippedEntryStream.CopyTo(ms);
                                var unzippedArray = ms.ToArray();

                                return Encoding.Default.GetString(unzippedArray);
                            }
                        }
                    }

                    return null;
                }
            }
        }
        
        //[Fact]
        //Ignored for general testing as it needs an archive node key
        public async void ShouldRetrieveUniswapV3TransactionFromChainAndValidateTracesDebugStorageAt()
        {
            //Uniswap v3 multicall
            await RetrieveTransactionFromChainAndCompareToExternalTraces("0x6669284f4072af03600f95bc4c1ed3499e1658dab87615cfd03775fea13a82b7", "EVM/Traces/0x6669284f4072af03600f95bc4c1ed3499e1658dab87615cfd03775fea13a82b7.json", "54532", null, true);

        }

    }
}


﻿using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.XUnitEthereumClients;
using Xunit; 
 // ReSharper disable ConsiderUsingConfigureAwait  
 // ReSharper disable AsyncConverter.ConfigureAwaitHighlighting

namespace Nethereum.Contracts.IntegrationTests.EncodingInputOutput
{
    [Collection(EthereumClientIntegrationFixture.ETHEREUM_CLIENT_COLLECTION_DEFAULT)]
    public class FunctionOutputDTOTests
    {
        private readonly EthereumClientIntegrationFixture _ethereumClientIntegrationFixture;

        public FunctionOutputDTOTests(EthereumClientIntegrationFixture ethereumClientIntegrationFixture)
        {
            _ethereumClientIntegrationFixture = ethereumClientIntegrationFixture;
        }


        public class TestOutputService
        {
            public static string ABI =
                @"[{'constant':false,'inputs':[],'name':'getData','outputs':[{'name':'birthTime','type':'uint64'},{'name':'userName','type':'string'},{'name':'starterId','type':'uint16'},{'name':'currLocation','type':'uint16'},{'name':'isBusy','type':'bool'},{'name':'owner','type':'address'}],'payable':false,'stateMutability':'nonpayable','type':'function'}]";

            public static string BYTE_CODE =
                "0x6060604052341561000f57600080fd5b6101c88061001e6000396000f3006060604052600436106100405763ffffffff7c01000000000000000000000000000000000000000000000000000000006000350416633bc5de308114610045575b600080fd5b341561005057600080fd5b61005861011a565b60405167ffffffffffffffff8716815261ffff808616604083015284166060820152821515608082015273ffffffffffffffffffffffffffffffffffffffff821660a082015260c06020820181815290820187818151815260200191508051906020019080838360005b838110156100da5780820151838201526020016100c2565b50505050905090810190601f1680156101075780820380516001836020036101000a031916815260200191505b5097505050505050505060405180910390f35b600061012461018a565b6000806000806001955060408051908101604052600481527f6a75616e0000000000000000000000000000000000000000000000000000000060208201529596600195508594506000935073de0b295669a9fd93d5f28d9ec85e40f4cb697bae92509050565b602060405190810160405260008152905600a165627a7a72305820ba7625d1c6f0f2844d32ad76e28729e80979f69cbd32d0589995f24cb969a6850029"; /*
            pragma solidity ^0.4.19;

            contract TestOutput {

                function getData() returns (uint64 birthTime, string userName, uint16 starterId, uint16 currLocation, bool isBusy, address owner ) {
                    birthTime = 1;
                    userName = "juan";
                    starterId = 1;
                    currLocation = 1;
                    isBusy = false;
                    owner = 0xde0b295669a9fd93d5f28d9ec85e40f4cb697bae;
                }
            }

            */

            private readonly Web3.Web3 web3;

            private readonly Contract contract;

            public TestOutputService(Web3.Web3 web3, string address)
            {
                this.web3 = web3;
                contract = web3.Eth.GetContract(ABI, address);
            }

            public Function GetFunctionGetData()
            {
                return contract.GetFunction("getData");
            }


            public Task<string> GetDataAsync(string addressFrom, HexBigInteger gas = null,
                HexBigInteger valueAmount = null)
            {
                var function = GetFunctionGetData();
                return function.SendTransactionAsync(addressFrom, gas, valueAmount);
            }

            public Task<GetDataDTO> GetDataAsyncCall()
            {
                var function = GetFunctionGetData();
                return function.CallDeserializingToObjectAsync<GetDataDTO>();
            }
        }

        [FunctionOutput]
        public class GetDataDTO
        {
            [Parameter("uint64", "birthTime", 1)] public ulong BirthTime { get; set; }

            [Parameter("string", "userName", 2)] public string UserName { get; set; }

            [Parameter("uint16", "starterId", 3)] public int StarterId { get; set; }

            [Parameter("uint16", "currLocation", 4)]
            public int CurrLocation { get; set; }

            [Parameter("bool", "isBusy", 5)] public bool IsBusy { get; set; }

            [Parameter("address", "owner", 6)] public string Owner { get; set; }
        }

        [Fact]
        public async void ShouldReturnFunctionOutputDTO()
        {
            var web3 = _ethereumClientIntegrationFixture.GetWeb3();
            var senderAddress = EthereumClientIntegrationFixture.AccountAddress;

            var contractReceipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(TestOutputService.ABI,
                TestOutputService.BYTE_CODE, senderAddress, new HexBigInteger(900000));
            var service = new TestOutputService(web3, contractReceipt.ContractAddress);
            var message = await service.GetDataAsyncCall();
            Assert.Equal(1, (int) message.BirthTime);
            Assert.Equal(1, message.CurrLocation);
            Assert.Equal(1, message.StarterId);
            Assert.False(message.IsBusy);
            Assert.Equal("juan", message.UserName);
            Assert.Equal("0xde0B295669a9FD93d5F28D9Ec85E40f4cb697BAe", message.Owner);
        }
    }
}

﻿using Nethereum.Contracts;
using Nethereum.JsonRpc.WebSocketStreamingClient;
using Nethereum.RPC.Reactive.Eth;
using Nethereum.RPC.Reactive.Eth.Subscriptions;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nethereum.WebSocketsStreamingTest
{
    internal class ExampleLogsUniswapSyncSubscription
    {
        private readonly string url;
        private readonly string rpcURl;
        StreamingWebSocketClient client;

        public ExampleLogsUniswapSyncSubscription(string url, string rpcURl)
        {
            this.url = url;
            this.rpcURl = rpcURl;
        }
        public async Task SubscribeAndRunAsync()
        {
            if (client == null)
            {
                client = new StreamingWebSocketClient(url);
                client.Error += Client_Error;
            }

            string uniSwapFactoryAddress = "0x5C69bEe701ef814a2B6a3EDD4B1652CB9cc5aA6f";
            var web3 = new Web3.Web3(rpcURl);


            string daiAddress = "0x6b175474e89094c44da98b954eedeac495271d0f";
            string wethAddress = "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2";

            var pairContractAddress = await web3.Eth.GetContractQueryHandler<GetPairFunction>()
                .QueryAsync<string>(uniSwapFactoryAddress,
                    new GetPairFunction() { TokenA = daiAddress, TokenB = wethAddress });

            var filter = Event<PairSyncEventDTO>.GetEventABI()
                .CreateFilterInput(new[] { pairContractAddress });

            var subscription = new EthLogsObservableSubscription(client);
            subscription.GetSubscriptionDataResponsesAsObservable().
                         Subscribe(log =>
                         {
                             try
                             {
                                 EventLog<PairSyncEventDTO> decoded = Event<PairSyncEventDTO>.DecodeEvent(log);
                                 if (decoded != null)
                                 {
                                     decimal reserve0 = Web3.Web3.Convert.FromWei(decoded.Event.Reserve0);
                                     decimal reserve1 = Web3.Web3.Convert.FromWei(decoded.Event.Reserve1);
                                     Console.WriteLine($@"Price={reserve0 / reserve1}");
                                 }
                                 else Console.WriteLine(@"Found not standard transfer log");
                             }
                             catch (Exception ex)
                             {
                                 Console.WriteLine(@"Log Address: " + log.Address + @" is not a standard transfer log:", ex.Message);
                             }
                         });

            await client.StartAsync();
            subscription.GetSubscribeResponseAsObservable().Subscribe(id => Console.WriteLine($"Subscribed with id: {id}"));
            await subscription.SubscribeAsync(filter);

            while (true) //pinging to keep alive infura
            {
                var handler = new EthBlockNumberObservableHandler(client);
                handler.GetResponseAsObservable().Subscribe(x => Console.WriteLine(x.Value));
                await handler.SendRequestAsync();
                Thread.Sleep(30000);
            }


        }


        private async void Client_Error(object sender, Exception ex)
        {
            Console.WriteLine("Client Error restarting...");
            // ((StreamingWebSocketClient)sender).Error -= Client_Error;
            ((StreamingWebSocketClient)sender).StopAsync().Wait();
            //Restart everything
            await SubscribeAndRunAsync();
        }
    }
}



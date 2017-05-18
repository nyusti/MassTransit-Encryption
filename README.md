# MassTransit-Encryption
Transparent encryption support for MassTransit built on Azure Key Vault

**CURRENTLY WORKING ONLY FOR AZURE SERVICE BUS**

## Usage
To configure the encryption use the `UseAzureKeyVaultEncryption` extension method on the bus factory configuration. You have to set the `encryptionKey` factory function when you want to encrypt outgoing messages.
Set the `keyResolver` factory function if you want to decrypt incoming messages.

The easiest way to acquire `IKey` and `IKeyResolver` types is via the [`Microsoft.Azure.KeyVault.Extensions.KeyVaultKeyResolver`](https://www.nuget.org/packages/Microsoft.Azure.KeyVault.Extensions) class.

## Key Vault permissions
For encryption `Wrap` permission is needed against Key Vault keys.
For decrpytion `Get` and `UnWrap` permission is needed against Key Vault keys.

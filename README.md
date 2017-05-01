# MassTransit-Encryption
Transparent encryption support for MassTransit built on Azure Key Vault

## Usage
To configure the encryption use the `UseAzureKeyVaultEncryption` extension method on the bus factory configuration. You have to set the `encryotionKey` factory function when you want to encrypt outgoing messages.
Set the `keyResolver` factory function if you want to decrpyt incoming messages.

## Key Vault permissions
For encryption `Wrap` permission is needed against Key Vault keys.
For decrpytion `Get` and `UnWrap` permission is needed against Key Vault keys.

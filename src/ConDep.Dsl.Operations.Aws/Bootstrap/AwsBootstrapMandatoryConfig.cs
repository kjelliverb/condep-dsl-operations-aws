﻿namespace ConDep.Dsl.Operations.Application.Local.Bootstrap.Aws
{
    public class AwsBootstrapMandatoryConfig : IOfferAwsBootstrapMandatoryConfig
    {
        private readonly AwsBootstrapMandatoryInputValues _values;

        public AwsBootstrapMandatoryConfig(string bootstrapId)
        {
            _values = new AwsBootstrapMandatoryInputValues(bootstrapId);
        }

        public AwsBootstrapMandatoryInputValues Values { get { return _values; } }

        public IOfferAwsBootstrapMandatoryConfig Credentials(string profileName)
        {
            _values.Credentials.UseProfile = true;
            _values.Credentials.ProfileName = profileName;
            return this;
        }

        public IOfferAwsBootstrapMandatoryConfig Credentials(string accessKey, string secretKey)
        {
            _values.Credentials.UseProfile = false;
            _values.Credentials.AccessKey = accessKey;
            _values.Credentials.SecretKey = secretKey;
            return this;
        }

        public IOfferAwsBootstrapMandatoryConfig KeyPair(string publicKeyName, string privateKeyFileLocation)
        {
            _values.PublicKeyName = publicKeyName;
            _values.PrivateKeyFileLocation = privateKeyFileLocation;
            return this;
        }

        public IOfferAwsBootstrapMandatoryConfig Region(string region)
        {
            _values.Region = region;
            return this;
        }
    }
}
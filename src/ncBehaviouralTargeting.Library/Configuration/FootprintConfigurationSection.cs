using System.Configuration;

namespace ncBehaviouralTargeting.Library.Configuration
{
    internal class FootprintConfigurationSection : ConfigurationSection
    {
        const string DisabledKey = "isDisabled";
        const string CookiesKey = "cookies";
        const string SegmentKey = "segment";
        const string SqlKey = "sql";
        const string MongoDbKey = "mongoDb";
        const string CustomerIoKey = "customerIo";
        const string EmailKey = "email";
        const string ProfileKey = "profile";

        [ConfigurationProperty(DisabledKey, DefaultValue = false, IsRequired = false)]
        public bool IsDisabled => (bool) this[DisabledKey];

        [ConfigurationProperty(CookiesKey)]
        public CookiesElement Cookies => (CookiesElement) this[CookiesKey];

        //[ConfigurationProperty(SegmentKey)]
        //public SegmentElement Segment => (SegmentElement) this[SegmentKey];

        [ConfigurationProperty(SqlKey)]
        public SqlElement Sql => (SqlElement) this[SqlKey];

        [ConfigurationProperty(MongoDbKey)]
        public MongoDbElement MongoDb => (MongoDbElement) this[MongoDbKey];

        [ConfigurationProperty(CustomerIoKey)]
        public CustomerIoElement CustomerIo => (CustomerIoElement) this[CustomerIoKey];

        [ConfigurationProperty(EmailKey)]
        public EmailElement Email => (EmailElement) this[EmailKey];

        [ConfigurationProperty(ProfileKey)]
        public ProfileElement Profile => (ProfileElement) this[ProfileKey];

        public class CookiesElement : ConfigurationElement
        {
        }

        public class SegmentElement : ConfigurationElement
        {
            const string WriteKeyKey = "writeKey";

            [ConfigurationProperty(WriteKeyKey, IsRequired = true)]
            public string WriteKey => (string) this[WriteKeyKey];
        }

        public class SqlElement : ConfigurationElement
        {
            const string ConnectionStringNameKey = "connectionStringName";

            [ConfigurationProperty(ConnectionStringNameKey, IsRequired = true)]
            public string ConnectionStringName => (string) this[ConnectionStringNameKey];
        }

        public class MongoDbElement : ConfigurationElement
        {
            const string ConnectionStringKey = "connectionString";
            const string DatabaseKey = "database";
            const string CollectionKey = "collection";

            [ConfigurationProperty(ConnectionStringKey, IsRequired = true)]
            public string ConnectionString => (string) this[ConnectionStringKey];

            [ConfigurationProperty(DatabaseKey, IsRequired = true)]
            public string Database => (string) this[DatabaseKey];

            [ConfigurationProperty(CollectionKey, IsRequired = true)]
            public string Collection => (string) this[CollectionKey];
        }

        public class CustomerIoElement : ConfigurationElement
        {
            const string SiteIdKey = "siteId";
            const string ApiKeyKey = "apiKey";

            [ConfigurationProperty(SiteIdKey, IsRequired = true)]
            public string SiteId => (string) this[SiteIdKey];

            [ConfigurationProperty(ApiKeyKey, IsRequired = true)]
            public string ApiKey => (string) this[ApiKeyKey];
        }

        public class EmailElement : ConfigurationElement
        {
            const string SenderKey = "sender";

            [ConfigurationProperty(SenderKey, IsRequired = true)]
            public string Sender => (string) this[SenderKey];
        }

        public class ProfileElement : ConfigurationElement
        {
            const string FieldsKey = "profileFields";

            [ConfigurationProperty(FieldsKey, IsDefaultCollection = true)]
            [ConfigurationCollection(typeof(ProfileFieldCollection), AddItemName = "profileField")]
            public ProfileFieldCollection ProfileFields => (ProfileFieldCollection) this[FieldsKey];

            public class ProfileFieldElement : ConfigurationElement
            {
                const string NameKey = "name";

                [ConfigurationProperty(NameKey, IsRequired = true)]
                public string Name => (string) this[NameKey];
            }

            public class ProfileFieldCollection : ConfigurationElementCollection
            {
                protected override ConfigurationElement CreateNewElement()
                {
                    return new ProfileFieldElement();
                }

                public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

                protected override object GetElementKey(ConfigurationElement element)
                {
                    return ((ProfileFieldElement) element).Name;
                }

                protected override string ElementName => "profileField";
            }
        }
    }
}

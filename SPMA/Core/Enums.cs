namespace SPMA.Core
{   
    public class Enums
    {
        public enum OrderType
        {
            External,
            Warranty,
            Internal
        }
        public enum DatabaseStatus
        {
            Error,
            Connecting,
            Connected
        }
        public enum ComponentType
        {
            Part,
            Assembly,
            Book
        }

        public enum BookListType
        {
            Standard,
            PlazmaCut,
            StoreBought
        }

        public enum ComponentSourceType
        {
            Standard,
            Saw,
            PlasmaIn,
            PlasmaOutWithEntrusted,
            PlasmaOutWithoutEntrusted,
            Purchase,
            StandardPurchase
        }

        public enum ProductionStateEnum
        {
            Idle = 1,
            Awaiting = 2,
            Tooling = 3,
            Assembly = 4,
            Finished = 5,
            PurchaseItem = 100
        }
        
        public enum LastSourceType
        {
            Standard = 0,
            //Reserve - nie używana rezerwa
            Reserve = 1,
            PlasmaIn = 2,
            PlasmaOutWithoutEntrustedMaterial = 3,
            PlasmaWithEntrustedMaterial = 4,
            Purchase = 5
        }
    }
}

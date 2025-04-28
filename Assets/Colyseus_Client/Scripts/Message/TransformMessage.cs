using System;

namespace Colyseus_Client
{
    [Serializable]
    public class TransformMessage
    {
        public string objectId = default;
        public string transform = default;

        public TransformMessage(string objectId, string transform)
        {
            this.objectId = objectId;
            this.transform = transform;
        }
    }
}
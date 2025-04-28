using System;

namespace Colyseus_Client
{
    [Serializable]
    public class RigidbodyMessage
    {
        public string objectId = default;
        public string rigidbody = default;

        public RigidbodyMessage(string objectId, string rigidbody)
        {
            this.objectId = objectId;
            this.rigidbody = rigidbody;
        }
    }
}

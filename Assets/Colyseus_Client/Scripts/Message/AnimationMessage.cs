using System;

namespace Colyseus_Client
{
    [Serializable]
    public class AnimationMessage
    {
        public string objectId = default;
        public string animation = default;

        public AnimationMessage(string objectId, string animation)
        {
            this.objectId = objectId;
            this.animation = animation;
        }
    }
}
namespace PGT.Core
{
    using UnityEngine;
    using Func;

    public class SyncMonoBehaviour : UnityEngine.MonoBehaviour, IPrivateEventDispatchable
    {
        public Future<Transform> GetTransformL()
        {
            return new Future<Transform>(() => base.transform);
        }

        public Future<Vector3> GetPositionL()
        {
            return new Future<Vector3>(() => base.transform.position);
        }

        public Future SetPositionL(Vector3 position)
        {
            return new Future(() => base.transform.position = position);
        }

        public Future SetPositionL(Future<Vector3> position)
        {
            return position.applyTo((Vector3 pos) => 
                {
                    base.transform.position = pos;
                });
        }

        public Future<Quaternion> GetRotationL()
        {
            return new Future<Quaternion>(() => base.transform.rotation);
        }

        public Future SetRotationL(Quaternion rotation)
        {
            return new Future(() => base.transform.rotation = rotation);
        }

        public Future SetRotationL(Future<Quaternion> rotation)
        {
            return rotation.applyTo((Quaternion q) =>
            {
                base.transform.rotation = q;
            });
        }
        

        public override string ToString()
        {
            return new Future<string>(base.ToString).bind();
        }

        public string instanceId()
        {
            return GetInstanceID().ToString();
        }
    }
}

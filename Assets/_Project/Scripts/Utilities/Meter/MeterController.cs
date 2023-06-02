using UnityEngine;

namespace Utilities.Meter
{
    public class MeterController : MonoBehaviour, IHaveMeter
    {
        [SerializeField] protected Meter _meter = new(100);
        public Meter GetMeter() => _meter;
    }
}

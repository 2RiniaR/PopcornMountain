using System;
using UniRx;
using UnityEngine;


namespace PopcornMountain.MainGameScene {
    public class FireManager : BaseMainGameManager {

        public const int minPower = 1;
        public const int maxPower = 3;


        private IntReactiveProperty firePower = new IntReactiveProperty(0);


        public IObservable<int> OnFirePowerChangedObservable { get { return firePower; } }
        public int FirePower { get { return firePower.Value; } }
        public float FireHeat { get { return ConvertPowerToHeat(FirePower); } }


        private float ConvertPowerToHeat(int power) {
            return 1f + (power - 1) * 2f;
        }


        public float GetConductionHeat(float distance) {
            return FireHeat / (distance >= 1f ? distance : 1f);
        }


        public void ChangeFirePower(int power) {
            power = power > maxPower ? maxPower : power;
            power = power < minPower ? minPower : power;
            firePower.Value = power;
        }

    }
}

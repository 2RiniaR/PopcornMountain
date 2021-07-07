using System;
using System.Collections.Generic;
using UniRx;


namespace PopcornMountain.MainGameScene {
    public abstract class PhaseSwitchablePresenterMonobehavior : BaseMainGameComponent {

        private List<Pair<IDisposable>> phaseObservers = new List<Pair<IDisposable>>();
        private PhaseManager phaseManager = null;
        protected bool isEnablePhase = false;
//
        private void Awake() {
            phaseManager = GetManager<PhaseManager>();
            int phaseNum = Enum.GetNames(typeof(GamePhase)).Length;
            for(int i=0; i<phaseNum; i++) phaseObservers.Add(new Pair<IDisposable>(null, null));
        }

        private void AddEnablePhase(GamePhase phase) {
            phaseObservers[(int)phase] = new Pair<IDisposable>(
                phaseManager.OnEnterPhase(phase).Subscribe(_ => isEnablePhase = true).AddTo(this),
                phaseManager.OnExitPhase(phase).Subscribe(_ => isEnablePhase = false).AddTo(this)
            );
        }

        protected void AddEnablePhases(params GamePhase[] phases) {
            foreach(var phase in phases) {
                AddEnablePhase(phase);
            }
            UpdatePhaseSwitchComponent();
        }

        private void DeleteEnablePhase(GamePhase phase) {
            phaseObservers[(int)phase].Previous.Dispose();
            phaseObservers[(int)phase].Current.Dispose();
            phaseObservers[(int)phase] = new Pair<IDisposable>(null, null);
        }

        protected void DeleteEnablePhases(params GamePhase[] phases) {
            foreach(var phase in phases) {
                DeleteEnablePhase(phase);
            }
            UpdatePhaseSwitchComponent();
        }

        private void UpdatePhaseSwitchComponent() {
            GamePhase currentPhase = phaseManager.CurrentGamePhase;
            isEnablePhase = phaseObservers[(int)currentPhase].Previous != null;
        }

    }
}

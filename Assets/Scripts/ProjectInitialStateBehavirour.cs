using Game;
using Project.States;
using UnityEngine;

namespace Project
{
    public class ProjectInitialStateBehavirour : MonoBehaviour
    {
        [SerializeField]
        private GameStartBehaviour _gameStart;

        private void Awake()
        {
            _gameStart.InitialStateFunc = () =>
            {
                return new GameInitializeState();
            };
        }
    }
}

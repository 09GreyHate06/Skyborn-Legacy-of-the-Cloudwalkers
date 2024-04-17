using UnityEngine;
using SLOTC.Utils.StateMachine;
using DuloGames.UI.Tweens;
using UnityEngine.InputSystem;
using System;

namespace SLOTC.Core.Player.States
{
    public class EmptyState : IState
    {
        private readonly string _uniqueID = Guid.NewGuid().ToString();
        public string GetID()
        {
            return _uniqueID;
        }

        public void OnEnter()
        {
        }

        public void OnExit()
        {

        }

        public void OnUpdate(float deltaTime)
        {

        }
    }
}
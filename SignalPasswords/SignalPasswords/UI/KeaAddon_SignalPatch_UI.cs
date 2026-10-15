using Gauge.Player;
using HarmonyLib;
using System;
using UnityEngine;

namespace SignalPasswords.UI
{
    public class KeaAddon_SignalPatch_UI : MonoBehaviour, ISimpleTextInputReciever, IPasswordLockUser
    {
        // Allow Console messages
        public static bool debugMode = false;

        // Interface method
        public bool SetText(string key, string value)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: SetText called"); }
            if (key == "signalPassword")
            {
                if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: SetText - signalPassword: " + value); }
                return true;
            }
            return false;
        }



        // Creates and sets up instance of PasswordUsableUI PasswordOnly prefab
        private PropPasswordUsableOptionsUI _menu;
        private void CreateUI()
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: CreateUI called"); }
            GameObject prefab = ObjectLoaderManager.GetAssets().miscEditingAssets.PropPasswordUsableOptionsUI;
            GameObject uiObject = Instantiate(prefab);

            _menu = uiObject.GetComponent<PropPasswordUsableOptionsUI>();
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: CreateUI - Menu component is null: " + (_menu == null)); }
            if (_menu == null)
                return;
            _menu.Setup(this, new PasswordActivatorData(), gameObject, PasswordUsableUIDisplayType.PasswordOnly);
        }



        // Spawns the UI on the right side of the vanilla UI, relative to the rotation
        private void RefreshMenuPosition(Transform parentPoint)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: RefreshMenuPosition called"); }
            _menu.transform.position = parentPoint.position;
            _menu.transform.LookAt(PlayerManager.GetHead().transform.position);
            _menu.transform.localEulerAngles = new Vector3(-10f, _menu.transform.localEulerAngles.y, 0f);

            Quaternion rotation = _menu.transform.rotation;
            _menu.transform.position = parentPoint.position + rotation * Vector3.right * -1.15f + rotation * Vector3.forward * 0.1f;

            _menu.transform.localScale = Vector3.one * 0.7f;
            _menu.transform.Translate(Vector3.up * 0.3f, Space.World);
        }



        // Creates the UI object if not null and sets signalPassMenuOpen to true, then sets the location to spawn
        public bool Show(Transform signalTransform)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: Show called"); }
            if (_menu == null)
            {
                CreateUI();
                signalPassMenuOpen = true;
            }
            RefreshMenuPosition(signalTransform);
            return true;
        }



        // Reads password value from KeaAddon_Signal._signalPassword
        public string GetPasswordValue()
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: GetPasswordValue called"); }
            var field = AccessTools.Field(typeof(KeaAddon_Signal), "_signalPassword");
            return (string)field.GetValue(GetComponent<KeaAddon_Signal>()) ?? "";
        }



        // Stores password value in KeaAddon_Signal._signalPassword
        public void SetPasswordValue(string value)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: SetPasswordValue - value: " + value); }
            var field = AccessTools.Field(typeof(KeaAddon_Signal), "_signalPassword");
            field.SetValue(GetComponent<KeaAddon_Signal>(), value);
        }



        // Opens or closes the UI menu based on signalPassMenuOpen bool
        public bool signalPassMenuOpen = false;
        public void OpenMenu(bool open)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch_UI :: OpenMenu called: " + open); }
            if (!open)
            {
                signalPassMenuOpen = false;
                if (_menu != null)
                {
                    AccessTools.Method(typeof(KeaAddon_Signal), "UpdateSaveString").Invoke(GetComponent<KeaAddon_Signal>(), null);
                    Destroy(_menu.gameObject);
                    _menu = null;
                }
            }
        }



        // Unity.OnDestroy - to remove UI on destroyed signal object
        private void OnDestroy()
        {
            if (_menu != null)
            {
                Destroy(_menu.gameObject);
                _menu = null;
            }
        }

    }
}

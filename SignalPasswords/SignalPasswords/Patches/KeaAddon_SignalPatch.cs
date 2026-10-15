using Gauge.Track;
using HarmonyLib;
using SignalPasswords.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace SignalPasswords.Patches
{

    [HarmonyPatch]
    public class KeaAddon_SignalPatch
    {
        // Allow Console messages
        public static bool debugMode = false;


        [HarmonyPatch(typeof(KeaAddon_Signal), "ResetSingnalData")]
        [HarmonyPrefix]

        // Line 432
        public static void ResetSingnalData_Prefix(KeaAddon_Signal __instance,ref string ____signalPassword)
        {
            string valueString = (string)AccessTools.Field(typeof(KeaAddon_ValueReciever), "valueString").GetValue(__instance);
            if (string.IsNullOrEmpty(valueString))
            {
                Console.WriteLine("ResetSingnalData_Prefix - Empty valueString; preserving password: " + ____signalPassword);
                return;
            }
            SignalSaveDataSet data = KeaAddon_Signal.GetDataFromSave(valueString);
            ____signalPassword = (string)AccessTools.Field(typeof(SignalSaveDataSet), "signalPassword").GetValue(data);
            Console.WriteLine("ResetSingnalData_Prefix - restored password: " + ____signalPassword);
        }



        [HarmonyPatch(typeof(KeaAddon_Signal), "UpdateLight")]
        [HarmonyPrefix]

        // Line 636
        // Injecting local method TriggerPasswordMessage to send out passwords on _signalOn change
        static bool UpdateLight_Prefix(ISignalHead ____signalHead, ref bool ____signalOn, string ____signalPassword, KeaAddon_Signal __instance, MeshFilter ___lightRef, Mesh[] ___lightMeshes)
        {
            // Cyclical, keeping commented is the best;; if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: UpdateLight_Prefix called"); }
            int value = (int)AccessTools.Field(typeof(KeaAddon_ValueReciever), "value").GetValue(__instance);

            if (____signalHead != null)
            {
                if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: UpdateLight_Prefix - _signalHead !null " + ____signalHead); }
                ____signalHead.SetState((SignalState)value);
                return false;
            }
            ___lightRef.sharedMesh = ___lightMeshes[value];

            if (!____signalOn && (float)value != 0f)
            {
                ____signalOn = true;
                TriggerPasswordMessage(__instance);
                if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: UpdateLight_Prefix signal ON"); }
            }
            if (____signalOn && (float)value == 0f)
            {
                ____signalOn = false;
                TriggerPasswordMessage(__instance);
                if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: UpdateLight_Prefix signal OFF"); }
            }
            return false;
        }



        // Local Method
        // Transmitting Password to Listeners
        private static void TriggerPasswordMessage(KeaAddon_Signal signal)
        {
            string password = (string)AccessTools.Field(typeof(KeaAddon_Signal), "_signalPassword").GetValue(signal);
            if (string.IsNullOrEmpty(password))
            {
                if (debugMode) {Console.WriteLine("ERROR: _signalPassword field was empty!");}
                return;
            }
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: TriggerPasswordMessage - _signalPassword: " + password); }
            DynamicMapDataManager.TriggerPropMessage("lever", password, signal.gameObject);
        }



        [HarmonyPatch(typeof(KeaAddon_Signal), "GetDataFromSave")]
        [HarmonyPrefix]

        // Line 90
        // Loads up object attributes from save
        static bool GetDataFromSave_Prefix(string saveSave, ref SignalSaveDataSet __result)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: GetDataFromSave called"); }
            SignalSaveDataSet result = default(SignalSaveDataSet);
            if (saveSave.IsNullOrEmpty())
            {
                __result = result;
                return false;
            }
            if (debugMode) { Console.WriteLine("GetDataFromSave input: " + saveSave); }
            try
            {
                string[] array = saveSave.Split(' ', StringSplitOptions.None);
                result.signalVersion = 1;
                result.towardsNext = bool.Parse(array[0]);
                result.redRange = int.Parse(array[1]);
                result.orangeRange = int.Parse(array[2]);
                result.trackID = int.Parse(array[3]);
                if (array.Length > 4)
                {
                    result.tempBlockAutomation = bool.Parse(array[4]);
                }
                if (array.Length > 5)
                {
                    result.signalVersion = int.Parse(array[5]);
                }
                if (array.Length > 6)
                {
                    result.id = array[6];
                }
                if (array.Length > 7)
                {
                    result.towardsNextAltSource = bool.Parse(array[7]);
                }
                if (array.Length > 8)
                {
                    result.useAltDetectionSourceTrack = bool.Parse(array[8]);
                }
                if (array.Length > 9)
                {
                    result.soureTrackID_alt = int.Parse(array[9]);
                }
                if (array.Length > 10)
                {
                    AccessTools.Field(typeof(SignalSaveDataSet), "signalPassword").SetValueDirect(__makeref(result), array[10]);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("invalid signal data: " + saveSave);
                Debug.LogException(exception);
            }
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: GetDataFromSave - signalPassword: " + (string)AccessTools.Field(typeof(SignalSaveDataSet), "signalPassword").GetValue(result)); }
            __result = result;
            return false;
        }



        [HarmonyPatch(typeof(KeaAddon_Signal), "GetSaveStringFromData")]
        [HarmonyPrefix]

        // Line 143
        // Creates save file data string from object attributes
        static bool GetSaveStringFromData_Prefix(SignalSaveDataSet saveData, ref string __result)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: GetSaveStringFromData called"); }
            string saveString = string.Concat(new string[]
            {
                saveData.towardsNext.ToString(),
                " ",
                saveData.redRange.ToString(),
                " ",
                saveData.orangeRange.ToString(),
                " ",
                saveData.trackID.ToString(),
                " ",
                saveData.tempBlockAutomation.ToString(),
                " ",
                saveData.signalVersion.ToString(),
                " ",
                saveData.id,
                " ",
                saveData.towardsNextAltSource.ToString(),
                " ",
                saveData.useAltDetectionSourceTrack.ToString(),
                " ",
                saveData.soureTrackID_alt.ToString(),
                " ",
                (string)AccessTools.Field(typeof(SignalSaveDataSet), "signalPassword").GetValue(saveData)
            });
            if (debugMode) { Console.WriteLine("GetSaveStringFromData output: " + saveString); }
            __result = saveString;
            return false;
        }



        [HarmonyPatch(typeof(KeaAddon_Signal), "UpdateSaveString")]
        [HarmonyPrefix]

        // Line 196
        // Saving extra variable to save file
        static bool UpdateSaveString_Prefix(KeaAddon_Signal __instance, TrackPathObject ____sourceTrack, TrackPathObject ____sourceTrackAlt, SignalObjectData ___data, bool ____tempBlockAutomation, string ____signalPassword)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: UpdateSaveString called"); }
            
            if (____sourceTrack == null)
            {
                AccessTools.Field(typeof(KeaAddon_ValueReciever), "valueString").SetValue(__instance, "");
                return false;
            }

            SignalSaveDataSet saveData = new SignalSaveDataSet
            {
                towardsNext = ___data.towardsNext,
                redRange = ___data.redRange,
                orangeRange = ___data.orangeRange,
                trackID = ____sourceTrack.ID,
                tempBlockAutomation = ____tempBlockAutomation,
                signalVersion = ___data.signalVersion,
                id = ___data.id,
                towardsNextAltSource = ___data.towardsNextAltSource,
                useAltDetectionSourceTrack = ___data.useAltDetectionSourceTrack,
                soureTrackID_alt = ((____sourceTrackAlt != null) ? ____sourceTrackAlt.ID : 0)
            };
            // Latewrite of signalPasswords due to preload variable creation
            AccessTools.Field(typeof(SignalSaveDataSet), "signalPassword").SetValueDirect(__makeref(saveData), ____signalPassword);
            // Copy saveData to valueString
            AccessTools.Field(typeof(KeaAddon_ValueReciever), "valueString").SetValue(__instance, KeaAddon_Signal.GetSaveStringFromData(saveData));
            return false;
        }



        [HarmonyPatch(typeof(KeaAddon_Signal), "OnHoverAction")]
        [HarmonyPrefix]

        // Line 220
        // Handles the opening of the password UI 
        private static bool OnHoverAction_Prefix(KeaAddon_Signal __instance, InputType _input)
        {
            if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: OnHoverAction_Prefix - " + _input); }
            if (_input == InputType.Trackpad_Left)
            {
                KeaAddon_SignalPatch_UI patchUI = __instance.GetComponent<KeaAddon_SignalPatch_UI>();

                if (patchUI != null && patchUI.signalPassMenuOpen)
                {
                    patchUI.OpenMenu(false);
                }
                else if (!(bool)AccessTools.Field(typeof(KeaAddon_Signal), "_editModeActive").GetValue(__instance))
                {
                    if (patchUI == null)
                    {
                        patchUI = __instance.gameObject.AddComponent<KeaAddon_SignalPatch_UI>();
                    }

                    if (debugMode) { Console.WriteLine("KeaAddon_SignalPatch :: OnHoverAction_Prefix - signalPassMenuOpen: " + patchUI.signalPassMenuOpen); }
                    patchUI.Show(__instance.transform);
                }
            }
            return true;
        }



        [HarmonyPatch(typeof(KeaAddon_Signal), "OnPlace")]
        [HarmonyPostfix]

        // Line 640
        // Responsible to close the modded UI on Q press
        private static void OnPlace_Postfix(KeaAddon_Signal __instance)
        {
            KeaAddon_SignalPatch_UI patchUI = __instance.GetComponent<KeaAddon_SignalPatch_UI>();
            if (patchUI != null && patchUI.signalPassMenuOpen)
            {
                patchUI.OpenMenu(false);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.Udon;
using VRC.Udon.Common;
using VRC.Udon.Common.Interfaces;
using Object = UnityEngine.Object;

namespace Prism.BehaviourEditor
{
    /*
     * USB : UdonSharpBehaviour
     */

    public static class BehaviourFieldUtil
    {
        #region Utility

        static IUdonVariable CreateUdonVariable(string symbolName, object value, System.Type type)
        {
            System.Type udonVariableType = typeof(UdonVariable<>).MakeGenericType(type);
            return (IUdonVariable) Activator.CreateInstance(udonVariableType, symbolName, value);
        }

        static IUdonSymbolTable GetSymbolTable(UdonBehaviour udonBehaviour)
        {

            if (!udonBehaviour || !(udonBehaviour.programSource is UdonSharpProgramAsset))
            {
                throw new Exception("ProgramSource is not an UdonSharpProgramAsset");
            }

            var programAsset = (UdonSharpProgramAsset) udonBehaviour.programSource;
            if (!programAsset)
            {
                throw new Exception("UdonBehaviour has no UdonSharpProgramAsset");
            }

            programAsset.UpdateProgram();

            var program = programAsset.GetRealProgram();
            if (program?.SymbolTable == null)
            {
                throw new Exception("UdonBehaviour has no public variables");
            }

            return program.SymbolTable;
        }

        public static List<UdonSharpBehaviour> GetAllUSB(bool includeInacitve)
        {
            var behaviours = new List<UdonSharpBehaviour>();
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                behaviours.AddRange(root.GetComponentsInChildren<UdonSharpBehaviour>(includeInacitve));
            }

            return behaviours;
        }

        public static FieldInfo[] GetUSBPublicFields(UdonSharpBehaviour behaviour)
        {
            var type = behaviour.GetType();
            return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        }

        public static bool SetVariableValue(UdonSharpBehaviour behaviour, FieldInfo field, object value)
        {
            var udon = behaviour.GetComponent<UdonBehaviour>();

            ((UdonSharpProgramAsset) udon.programSource).UpdateProgram();


            Undo.RecordObject(udon, "Modify variable");
            if (!udon.publicVariables.TrySetVariableValue(field.Name, value))
            {
                var symbolTable = GetSymbolTable(udon);
                var symbolType = symbolTable.GetSymbolType(field.Name);
                if (!udon.publicVariables.TryAddVariable(CreateUdonVariable(field.Name,
                        value,
                        symbolType)))
                {
                    Debug.LogError($"Failed to set public variable '{field.Name}' value");
                }
            }

            udon.SetProgramVariable(field.Name, value);
            return true;
        }

        public static void SaveChangedUSBPrefab(UdonSharpBehaviour behaviour)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(behaviour))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(behaviour);
            }
        }

        #endregion
    }
}
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


public static class SignalSaveDataSetPatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = new[] { "Assembly-CSharp.dll" };

    //  Patching
    //      - SignalSaveDataSet with the extra var: public string .signalPassword
    //      - KeaAddon_Signal with the extra var: private string _signalPassword
    public static void Patch(AssemblyDefinition assembly)
    {
        FieldDefinition field;
        // SignalSaveDataSet
        TypeDefinition SignalDataSet = assembly.MainModule.GetType("SignalSaveDataSet");
        if (SignalDataSet == null) throw new Exception("Could not find SignalSaveDataSet!");

        // skip if exists
        if (!SignalDataSet.Fields.Any(f => f.Name == "signalPassword"))
        {
            field = new FieldDefinition("signalPassword", FieldAttributes.Public, assembly.MainModule.TypeSystem.String);
            SignalDataSet.Fields.Add(field);
        }

        // KeaAddon_Signal _signalPassword, _signalOn
        TypeDefinition KeaAddonSignal = assembly.MainModule.GetType("KeaAddon_Signal");
        if (KeaAddonSignal == null) throw new Exception("Could not find KeaAddon_Signal!");

        // skip if exists
        if (!KeaAddonSignal.Fields.Any(f => f.Name == "_signalPassword"))
        {
            field = new FieldDefinition("_signalPassword", FieldAttributes.Private, assembly.MainModule.TypeSystem.String);
            KeaAddonSignal.Fields.Add(field);
        }

        // skip if exists
        if (!KeaAddonSignal.Fields.Any(f => f.Name == "_signalOn"))
        {
            field = new FieldDefinition("_signalOn", FieldAttributes.Private, assembly.MainModule.TypeSystem.Boolean);
            KeaAddonSignal.Fields.Add(field);
        }

        // Verify changes
        Console.WriteLine("signalPassword created: " + SignalDataSet.Fields.Any(f => f.Name == "signalPassword"));
        Console.WriteLine("_signalPassword created: " + KeaAddonSignal.Fields.Any(f => f.Name == "_signalPassword"));
        Console.WriteLine("_signalOn created: " + KeaAddonSignal.Fields.Any(f => f.Name == "_signalOn"));

    }

    private static TypeDefinition FindType(
        IEnumerable<TypeDefinition> types,
        string name)
    {
        foreach (TypeDefinition type in types)
        {
            if (type.Name == name)
                return type;

            TypeDefinition nested = FindType(
                type.NestedTypes, name
            );

            if (nested != null)
                return nested;
        }

        return null;
    }
}

namespace Injector
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using Mono.Collections.Generic;
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public static class Injector
    {
        public static void Inject(AssemblyDefinition game, string outputPath)
        {
            TypeDefinition global = game.MainModule.GetType(string.Empty, "Global");

            if (global == null)
            {
                Console.WriteLine("Global not found");
                Console.Read();
                return;
            }

            MethodDefinition globalStart = global.Methods.FirstOrDefault(method => method.Name == "Awake");

            if (globalStart == null)
            {
                Console.WriteLine("Global.Awake not found");
                Console.Read();
                return;
            }

            ILProcessor             p = globalStart.Body.GetILProcessor();
            Collection<Instruction> i = p.Body.Instructions;

            /*
            i.Insert(0, p.Create(OpCodes.Nop));
            i.Insert(1, p.Create(OpCodes.Nop));
            i.Insert(2, p.Create(OpCodes.Call, Util.ImportMethod<Assembly>(game, "GetExecutingAssembly")));
            i.Insert(3, p.Create(OpCodes.Callvirt, Util.ImportMethod<Assembly>(game, "get_Location", typeof(string))));
            i.Insert(4, p.Create(OpCodes.Call, Util.ImportMethod<System.IO.Path>(game, "GetDirectoryName")));
            */
            int index = 0;

            // Assembly.LoadFrom(Application.dataPath + "/Mods/ModLoader.dll")
            // i.Insert(0, p.Create(OpCodes.Call, Util.ImportMethod<Application>(game, "get_dataPath")));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Call,
                              ImportMethod<Assembly>(game, "GetExecutingAssembly")));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Callvirt,
                              ImportMethod<Assembly>(game, "get_Location")));

            // i.Insert(2, p.Create(OpCodes.Call, Util.ImportMethod<Path>(game, "GetDirectoryName", typeof(string))));
            Type[] types = {typeof(string)};
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Call,
                              game.MainModule.ImportReference(typeof(Path).GetMethod("GetDirectoryName", types))));

            // i.Insert(3, p.Create(OpCodes.Stloc_0));
            i.Insert(index++, p.Create(OpCodes.Ldstr, "/ModLoader.dll"));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Call,
                              ImportMethod<string>(
                                                   game,
                                                   "Concat",
                                                   typeof(string),
                                                   typeof(string))));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Call,
                              ImportMethod<Assembly>(game, "LoadFrom", typeof(string))));

            // .GetType("spaar.ModLoader.Internal.Activator()
            i.Insert(index++, p.Create(OpCodes.Ldstr, "ModLoader.Activator"));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Callvirt,
                              ImportMethod<Assembly>(game, "GetType", typeof(string))));

            // .GetMethod("Activate")
            i.Insert(index++, p.Create(OpCodes.Ldstr, "Activate"));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Callvirt,
                              ImportMethod<Type>(game, "GetMethod", typeof(string))));

            // .Invoke(null, null);
            i.Insert(index++, p.Create(OpCodes.Ldnull));
            i.Insert(index++, p.Create(OpCodes.Ldnull));
            i.Insert(
                     index++,
                     p.Create(
                              OpCodes.Callvirt,
                              ImportMethod<MethodBase>(
                                                       game,
                                                       "Invoke",
                                                       typeof(object),
                                                       typeof(object[]))));
            i.Insert(index++, p.Create(OpCodes.Pop));

            game.Write(outputPath);
        }

        public static MethodReference ImportMethod<T>(AssemblyDefinition assembly, string name)
        {
            return assembly.MainModule.ImportReference(typeof(T).GetMethod(name, Type.EmptyTypes));
        }

        public static MethodReference ImportMethod<T>(AssemblyDefinition assembly, string name, params Type[] types)
        {
            return assembly.MainModule.ImportReference(typeof(T).GetMethod(name, types));
        }

        public static MethodReference ImportMethod(
        AssemblyDefinition assembly,
        string             type,
        string             method,
        params Type[]      types)
        {
            TypeReference reference = assembly.MainModule.Types.First(t => t.Name                    == type);
            return assembly.MainModule.ImportReference(reference.Resolve().Methods.First(m => m.Name == method));
        }
    }
}
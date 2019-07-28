//using SDG.Unturned;
//using Steamworks;
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;

//namespace ItemRestrictorAdvanced
//{
//    class Class
//    {
//        public void Start()
//        {
//            SteamChannel channel = EffectManager.instance.channel;
//            Component component = channel.calls[131].component;
//            Type type = typeof(MyEffectManager);
//            MethodInfo method = type.GetMethod("tellUIEffectParams");
//            //channel.calls[131].types
//            ParameterInfo[] parameters = method.GetParameters();
//            List<Type> typesList = new List<Type>();
//            foreach (var typ in parameters)
//                typesList.Add(typ.ParameterType);
//            Type[] types = typesList.ToArray();
//            channel.calls[132] = new SteamChannelMethod(component, method, types, channel.calls[131].attribute);
//            //channel.build();
//            Console.WriteLine("CALLS-IN-----Class---------------------------------");
//            for (byte i = 0; i < EffectManager.instance.channel.calls.Length; i++)
//            {
//                Console.WriteLine($"{i}. {EffectManager.instance.channel.calls[i].method.Name}");
//            }

//            //foreach (var call in EffectManager.instance.channel.calls)
//            //{
//            //    Console.WriteLine(call == null);
//            //}
//            Console.WriteLine();
//        }
//        [SteamCall(ESteamCallValidation.ONLY_FROM_SERVER)]
//        public void tellUIEffectParams(
//        CSteamID steamID,
//        ushort id,
//        short key,
//        params string[] args)
//        {
//            object[] newArgs = new object[args.Length];
//            for (byte i = 0; i < newArgs.Length; i++)
//                newArgs[i] = args[i];
//            EffectManager.createAndFormatUIEffect(id, key, newArgs);
//        }
//    }
//    class MyEffectManager : EffectManager
//    {
//        public void sendUIEffect(ushort id, short key, CSteamID steamID, bool reliable, params string[] arg)
//        {
//            object[] newArgs = new object[arg.Length + 2];
//            newArgs[0] = id;
//            newArgs[1] = key;
//            for (byte i = 2; i < newArgs.Length; i++)
//                newArgs[i] = arg[i-2];
//            Console.WriteLine("CALLS-IN-----MyEffectManager---------------------------------");
//            for (byte i = 0; i < EffectManager.instance.channel.calls.Length; i++)
//            {
//                Console.WriteLine($"{i}. {EffectManager.instance.channel.calls[i].method.Name}");
//            }

//            SteamChannel channel = instance.channel;
//            Component component = channel.calls[131].component;
//            Type type = typeof(MyEffectManager);
//            MethodInfo method = type.GetMethod("tellUIEffectParams");
//            //channel.calls[131].types
//            ParameterInfo[] parameters = method.GetParameters();
//            List<Type> typesList = new List<Type>();
//            foreach (var typ in parameters)
//                typesList.Add(typ.ParameterType);
//            Type[] types = typesList.ToArray();
//            channel.calls[132] = new SteamChannelMethod(component, method, types, channel.calls[131].attribute);

//            //send("tellUIEffect4Args", steamID, (ESteamPacket) (!reliable ? 16 : 15), (object) id, (object) key, (object) arg0, (object) arg1, (object) arg2, (object) arg3);
//            instance.channel.send("tellUIEffectParams", steamID, (ESteamPacket)(!reliable ? 16 : 15), newArgs);
//        }
//        [SteamCall(ESteamCallValidation.ONLY_FROM_SERVER)]
//        public void tellUIEffectParams(
//        CSteamID steamID,
//        ushort id,
//        short key,
//        params string[] args)
//        {
//            object[] newArgs = new object[args.Length];
//            for (byte i = 0; i < newArgs.Length; i++)
//                newArgs[i] = args[i];
//            EffectManager.createAndFormatUIEffect(id, key, newArgs);
//        }
//    }
//}

using System;
using Il2CppInterop.Runtime;

namespace KogamaToolsX.Utils;
internal static class Il2CppEnum
{
    internal static unsafe Il2CppSystem.Object ToIl2Cpp<T>(this T value) where T : Enum
    {
        IntPtr obj = IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<T>.NativeClassPtr);

        *(long*)((byte*)obj + 8) = Convert.ToInt64(value);

        return new Il2CppSystem.Object(obj);
    }

    internal static unsafe T ToEnum<T>(this Il2CppSystem.Object il2CppObject) where T : Enum
    {
        long enumValue = *(long*)((byte*)il2CppObject.Pointer + 8);

        return (T)Enum.ToObject(typeof(T), enumValue);
    }
}

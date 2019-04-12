using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UsefulLib.MVVMClasses
{
    /// <summary>
    /// Inherit, create string properies(with INotified) and create resx files with same var names and appropriate language file name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LocalizationManagerBase<T> : NotifiedBase where T: LocalizationManagerBase<T>
    {
        ResourceManager resourceManager;
        static T localizationManager;
        public static T GetLocalizationManager()
        {
            if (localizationManager != null)
                return localizationManager;
            else
            {
                var ctor = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                return localizationManager = (T)ctor.Invoke(null);
            }
        }

        public string CurrentCulture
        {
            set
            {
                CultureInfo cultureInfo = new CultureInfo(value);
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Notify();
            }
        }

        protected LocalizationManagerBase(string resourceName, Assembly assembly)
        {
            resourceManager = new ResourceManager(resourceName, assembly);
            PropertyChanged += PropertyChangedHandler;
        }

        protected string GetString([CallerMemberName]string str = "")
        {
            try { return resourceManager.GetString(str); }
            catch { return string.Empty; }
        }

        void PropertyChangedHandler(object o, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentCulture")
            {
                foreach (var prop in typeof(T).GetProperties())
                    if (prop.Name != "CurrentCulture")
                        Notify(prop.Name);
            }
        }
    }
}

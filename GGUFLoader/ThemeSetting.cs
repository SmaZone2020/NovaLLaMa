using iNKORE.UI.WPF.Modern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iNKORE.UI.WPF.Modern.Controls; 
using System.Windows;

namespace GGUFLoader
{
    public class ThemeSetting
    {
        public static bool Dark = false;
        public static string GetRequestedTheme()
        {
            // 获取应用程序的资源字典
            var resourceDictionary = Application.Current.Resources;

            // 遍历合并的字典，找到 ThemeResources
            foreach (var dictionary in resourceDictionary.MergedDictionaries)
            {
                if (dictionary is ThemeResources themeResources)
                {
                    // 返回当前的 RequestedTheme
                    return themeResources.RequestedTheme.ToString();
                }
            }

            return null; // 如果没有找到则返回 null
        }

        // 示例函数来设置 ThemeResources 的 RequestedTheme 属性
        public static void SetRequestedTheme(string theme)
        {
            // 获取应用程序的资源字典
            var resourceDictionary = Application.Current.Resources;

            // 遍历合并的字典，找到 ThemeResources
            foreach (var dictionary in resourceDictionary.MergedDictionaries)
            {
                if (dictionary is ThemeResources themeResources)
                {
                    // 设置 RequestedTheme 为新的值（例如 "Light" 或 "Dark"）
                    themeResources.RequestedTheme = (theme == "Light")
                        ? ApplicationTheme.Light
                        : ApplicationTheme.Dark;
                }
            }
        }
    }
}

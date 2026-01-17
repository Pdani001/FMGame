using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ReFMGame.GameHelper
{
    public static class MethodHelper
    {
        public static double Map(this double val, double min, double max, double toMin, double toMax)
        {
            val = Math.Min(max, Math.Max(min, val));
            return (val - min) * (toMax - toMin) / (max - min) + toMin;
        }
        public static long ExtractBits(this long value, int from, int to)
        {
            long mask = (1 << (to - from + 1)) - 1;
            return (value >> from) & mask;
        }

        public static string[] WrapString(this BitmapFont font, string text, float maxWidth)
        {
            var paragraphs = text.Split('\n');
            List<string> lines = new List<string>();

            foreach (var paragraph in paragraphs)
            {
                var paragraphTrimmed = paragraph.Trim().Replace("\t", "    ");
                if (string.IsNullOrEmpty(paragraphTrimmed))
                {
                    lines.Add(paragraphTrimmed);
                    continue;
                }

                string currentLine = "";
                var words = paragraphTrimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var word in words)
                {
                    float wordWidth = font.MeasureString(word).Width;

                    if (font.MeasureString(currentLine + (currentLine.Length > 0 ? " " : "") + word).Width <= maxWidth)
                    {
                        currentLine += (currentLine.Length > 0 ? " " : "") + word;
                    }
                    else if (wordWidth <= maxWidth)
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        foreach (char c in word)
                        {
                            float charWidth = font.MeasureString(c.ToString()).Width;

                            if (font.MeasureString(currentLine + c).Width > maxWidth)
                            {
                                lines.Add(currentLine);
                                currentLine = "";
                            }

                            currentLine += c;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                    lines.Add(currentLine);
            }

            return lines.ToArray();
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public static string GetPath(string name) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
        public static void SaveJson<T>(string name, T json, JsonTypeInfo<T> typeInfo)
        {
            string jsonPath = GetPath(name);
            Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
            string jsonString = JsonSerializer.Serialize(json, typeInfo);
            File.WriteAllText(jsonPath, jsonString);
        }
        public static T EnsureJson<T>(string name, JsonTypeInfo<T> typeInfo) where T : new()
        {
            T json;
            string jsonPath = GetPath(name);

            if (File.Exists(jsonPath))
            {
                json = JsonSerializer.Deserialize(File.ReadAllText(jsonPath), typeInfo)!;
            }
            else
            {
                json = new T();
                string jsonString = JsonSerializer.Serialize(json, typeInfo);
                Directory.CreateDirectory(Path.GetDirectoryName(jsonPath)!);
                File.WriteAllText(jsonPath, jsonString);
            }

            return json;
        }
    }
}

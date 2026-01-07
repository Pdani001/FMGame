using MonoGame.Extended.BitmapFonts;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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

        public static int MeasureWrappedLineCount(this BitmapFont font, string text, float maxWidth)
        {
            int totalLines = 0;

            var paragraphs = text.Split('\n');

            foreach (var paragraph in paragraphs)
            {
                if (paragraph.Length == 0)
                {
                    totalLines++;
                    continue;
                }

                string currentLine = "";
                var words = paragraph.Split(' ');

                foreach (var word in words)
                {
                    // Try normal word append
                    string test = currentLine.Length == 0
                        ? word
                        : currentLine + " " + word;

                    if (font.MeasureString(test).Width <= maxWidth)
                    {
                        currentLine = test;
                    }
                    else
                    {
                        // Word itself fits on a new line
                        if (font.MeasureString(word).Width <= maxWidth)
                        {
                            totalLines++;
                            currentLine = word;
                        }
                        // Force-break by character (SAFE version)
                        else
                        {
                            foreach (char c in word)
                            {
                                string charTest = currentLine + c;

                                if (font.MeasureString(charTest).Width > maxWidth)
                                {
                                    totalLines++;
                                    currentLine = c.ToString();
                                }
                                else
                                {
                                    currentLine = charTest;
                                }
                            }
                        }
                    }
                }

                totalLines++;
            }

            return totalLines;
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
    }
}

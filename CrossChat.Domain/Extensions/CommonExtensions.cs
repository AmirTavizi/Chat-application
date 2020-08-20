using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrossChat.Domain.Extensions
{
    public static class CommonExtensions
    {
        //Converts a slug to original text
        public static string FromSlug(this string text)
        {
            if (text!=null)
            {
                text = text.Replace("-", " ");
            }
           
            
            return text;
        }
        //Converts a text to slug
        public static string ToSlug(this string text)
        {
            //text = text.Replace("-", "&&");

            text = text.Replace(" ", "-");

            return text;
        }
        //Converts a text to slug
        public static string ToSlugNonTurkish(this string text)
        {
            text = ConverTurkishToSlug(text);
            text = GenerateSlug(text);
            return text;
        }
        private static string ConverTurkishToSlug(string originalTex)
        {
            string turkishSpecialChars = "ığüşöçĞÜŞİÖÇ";
            string correspondingChars = "igusocGUSIOC";

            for (int i = 0; i < turkishSpecialChars.Length - 1; i++)
            {
                originalTex = originalTex.Replace(turkishSpecialChars[i], correspondingChars[i]);
            }
            return originalTex;
        }
        public static string GenerateSlug(this string phrase)
        {
            string str = phrase.RemoveAccent().ToLower();
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str;
        }

        public static string RemoveAccent(this string txt)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

    }
}

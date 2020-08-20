using System;
using System.Collections.Generic;
using System.Text;

namespace CrossChat.Domain.Common
{
    public static class ResultMessages
    {
        public static string NonExistingData = "Mevcut Olmayan Veri";
        public static string InValidOrNullParameter= "Geçersiz veya NULL Parametre";
        public static string NonConvertibleParameters = "Birbirine Dönüştürülemeyen Parametre";
        public static string AlreadyExistingData = "Zaten Ekli Veri";
        public static string IllegalState = "Uygun Durumda olmayan Veri";
        public static string NotCompletedBatchWork = "Çoklu İşlem Başarı İle Tamamlanamadı";
        public static string DataUptateError = "Veri Güncelleme Hatası";
        public static string SuccessFullyExeCuteCommand = "Başarıyla gerçekliştirildi.";



    }
}

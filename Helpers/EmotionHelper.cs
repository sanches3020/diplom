using Sofia.Web.Models;

namespace Sofia.Web.Helpers;

public static class EmotionHelper
{
    // ----------------------------------------------------
    // 1. Название настроения (string)
    // ----------------------------------------------------
    public static string GetMoodName(string mood)
    {
        return mood.ToLower() switch
        {
            "happy" => "Хорошее",
            "joy" => "Хорошее",

            "sad" => "Грустное",
            "sorrow" => "Грустное",

            "angry" => "Злое",
            "anger" => "Злое",

            "fear" => "Напуганное",
            "surprise" => "Удивлённое",
            "neutral" => "Нейтральное",

            _ => "Неизвестно"
        };
    }

    // ----------------------------------------------------
    // 2. Описание настроения (string)
    // ----------------------------------------------------
    public static string GetMoodDescription(string mood)
    {
        return mood.ToLower() switch
        {
            "happy" => "Питомец чувствует себя отлично",
            "joy" => "Питомец чувствует себя отлично",

            "sad" => "Питомец грустит",
            "sorrow" => "Питомец грустит",

            "angry" => "Питомец раздражён",
            "anger" => "Питомец раздражён",

            "fear" => "Питомец испытывает тревогу",
            "surprise" => "Питомец удивлён",
            "neutral" => "Нейтральное состояние",

            _ => "Настроение не определено"
        };
    }

    // ----------------------------------------------------
    // 3. Emoji эмоций (string)
    // ----------------------------------------------------
    public static string GetEmotionEmoji(string emotion)
    {
        return emotion.ToLower() switch
        {
            "happy" => "😊",
            "joy" => "😊",

            "sad" => "😢",
            "sorrow" => "😢",

            "angry" => "😡",
            "anger" => "😡",

            "fear" => "😨",
            "surprise" => "😲",
            "neutral" => "😐",

            _ => "❓"
        };
    }

    // ----------------------------------------------------
    // 4. Emoji эмоций (int)
    // ----------------------------------------------------
    public static string GetEmotionEmoji(int emotion)
    {
        return emotion switch
        {
            0 => "😊",
            1 => "😢",
            2 => "😨",
            3 => "😡",
            4 => "😲",
            5 => "🤢",
            _ => "❓"
        };
    }

    // ✔ ДОБАВЛЕНО: Emoji эмоций (EmotionType)
    public static string GetEmotionEmoji(EmotionType emotion)
        => GetEmotionEmoji((int)emotion);

    // ----------------------------------------------------
    // 5. Название эмоции (string)
    // ----------------------------------------------------
    public static string GetEmotionName(string emotion)
    {
        return emotion.ToLower() switch
        {
            "happy" => "Радость",
            "joy" => "Радость",

            "sad" => "Грусть",
            "sorrow" => "Грусть",

            "angry" => "Злость",
            "anger" => "Злость",

            "fear" => "Страх",
            "surprise" => "Удивление",
            "neutral" => "Нейтрально",

            _ => "Неизвестно"
        };
    }

    // ----------------------------------------------------
    // 6. Название эмоции (int)
    // ----------------------------------------------------
    public static string GetEmotionName(int emotion)
    {
        return emotion switch
        {
            0 => "Радость",
            1 => "Грусть",
            2 => "Страх",
            3 => "Злость",
            4 => "Удивление",
            5 => "Отвращение",
            _ => "Неизвестно"
        };
    }

    // ✔ ДОБАВЛЕНО: Название эмоции (EmotionType)
    public static string GetEmotionName(EmotionType emotion)
        => GetEmotionName((int)emotion);

    // ----------------------------------------------------
    // 7. Цвет эмоции (string)
    // ----------------------------------------------------
    public static string GetEmotionColor(string emotion)
    {
        return emotion.ToLower() switch
        {
            "happy" => "#FFD700",
            "joy" => "#FFD700",

            "sad" => "#1E90FF",
            "sorrow" => "#1E90FF",

            "angry" => "#FF4500",
            "anger" => "#FF4500",

            "fear" => "#8A2BE2",
            "surprise" => "#00CED1",
            "neutral" => "#A9A9A9",

            _ => "#CCCCCC"
        };
    }

    // ----------------------------------------------------
    // 8. Цвет эмоции (int)
    // ----------------------------------------------------
    public static string GetEmotionColor(int emotion)
    {
        return emotion switch
        {
            0 => "#FFD700",
            1 => "#1E90FF",
            2 => "#8A2BE2",
            3 => "#FF4500",
            4 => "#00CED1",
            5 => "#6B7280",
            _ => "#CCCCCC"
        };
    }

    // ✔ ДОБАВЛЕНО: Цвет эмоции (EmotionType)
    public static string GetEmotionColor(EmotionType emotion)
        => GetEmotionColor((int)emotion);

    // ----------------------------------------------------
    // 9. Доминирующая эмоция
    // ----------------------------------------------------
    public static EmotionType GetDominantEmotion(IEnumerable<EmotionType> emotions)
    {
        return emotions
            .GroupBy(e => e)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();
    }
}

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
            1 => "😢",
            2 => "😔",
            3 => "😐",
            4 => "😊",
            5 => "😄",
            6 => "😰",
            7 => "😌",
            8 => "🤩",
            9 => "😤",
            10 => "🙏",
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
            "verysad" => "Очень грустно",
            "sad" => "Грустно",
            "neutral" => "Нейтрально",
            "happy" => "Радостно",
            "veryhappy" => "Очень радостно",
            "anxious" => "Тревожно",
            "calm" => "Спокойно",
            "excited" => "Взволнованно",
            "frustrated" => "Раздражённо",
            "grateful" => "Благодарно",
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
            1 => "Очень грустно",
            2 => "Грустно",
            3 => "Нейтрально",
            4 => "Радостно",
            5 => "Очень радостно",
            6 => "Тревожно",
            7 => "Спокойно",
            8 => "Взволнованно",
            9 => "Раздражённо",
            10 => "Благодарно",
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
            "verysad" => "#1e3a8a",
            "sad" => "#3b82f6",
            "neutral" => "#64748b",
            "happy" => "#22c55e",
            "veryhappy" => "#16a34a",
            "anxious" => "#f59e0b",
            "calm" => "#14b8a6",
            "excited" => "#8b5cf6",
            "frustrated" => "#f97316",
            "grateful" => "#0ea5e9",
            _ => "#e2e8f0"
        };
    }

    // ----------------------------------------------------
    // 8. Цвет эмоции (int)
    // ----------------------------------------------------
    public static string GetEmotionColor(int emotion)
    {
        return emotion switch
        {
            1 => "#2563eb",
            2 => "#3b82f6",
            3 => "#64748b",
            4 => "#22c55e",
            5 => "#16a34a",
            6 => "#f59e0b",
            7 => "#14b8a6",
            8 => "#8b5cf6",
            9 => "#f97316",
            10 => "#0ea5e9",
            _ => "#e2e8f0"
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

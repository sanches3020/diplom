using Sofia.Web.Models;

namespace Sofia.Web.Services.Companion;

public static class CompanionMessages
{
    public static readonly Dictionary<EmotionType, string> EmotionMoodMap = new()
    {
        [EmotionType.VerySad] = "sad",
        [EmotionType.Sad] = "concerned",
        [EmotionType.Neutral] = "neutral",
        [EmotionType.Happy] = "happy",
        [EmotionType.VeryHappy] = "excited",
        [EmotionType.Anxious] = "worried",
        [EmotionType.Calm] = "peaceful",
        [EmotionType.Excited] = "energetic",
        [EmotionType.Frustrated] = "confused",
        [EmotionType.Grateful] = "loving"
    };

    public static readonly Dictionary<EmotionType, string[]> EmotionMessages = new()
    {
        [EmotionType.VerySad] = new[]
        {
            "😢 Я вижу, что тебе очень грустно... Хочешь обнять меня?",
            "💙 Ты не одинок, я здесь с тобой. Расскажи мне, что случилось?",
            "🤗 Давай вместе переживем это трудное время. Я поддержу тебя!"
        },
        [EmotionType.Sad] = new[]
        {
            "😔 Похоже, у тебя грустный день. Хочешь поговорить об этом?",
            "💙 Я чувствую твою грусть. Давай найдем что-то хорошее в этом дне?",
            "🤗 Иногда грусть — это нормально. Я рядом, чтобы поддержать тебя!"
        },
        [EmotionType.Neutral] = new[]
        {
            "😊 Привет! Как дела? Хочешь поиграть со мной?",
            "🐾 Я здесь! Расскажи мне, как прошел твой день?",
            "💫 Давай проведем время вместе! Что бы ты хотел сделать?"
        },
        [EmotionType.Happy] = new[]
        {
            "😄 Ура! Я вижу, что ты в хорошем настроении! Давай повеселимся!",
            "🎉 Твоя радость заразительна! Хочешь поиграть?",
            "✨ Когда ты счастлив, я тоже счастлив! Давай отпразднуем!"
        },
        [EmotionType.VeryHappy] = new[]
        {
            "🤩 Вау! Ты просто сияешь от счастья! Это прекрасно!",
            "🎊 Твоя радость просто невероятна! Давай поделимся этим настроением!",
            "🌟 Ты делаешь мир лучше своей улыбкой! Я так горжусь тобой!"
        },
        [EmotionType.Anxious] = new[]
        {
            "😰 Я чувствую твою тревогу... Давай сделаем глубокий вдох вместе?",
            "🤲 Ты в безопасности. Я здесь, чтобы помочь тебе успокоиться.",
            "💆‍♀️ Давай попробуем расслабиться. Я буду рядом с тобой."
        },
        [EmotionType.Calm] = new[]
        {
            "😌 Какая прекрасная тишина... Я чувствую твой покой.",
            "🧘‍♀️ Ты выглядишь таким спокойным. Это очень красиво.",
            "🌿 Твоя внутренняя гармония вдохновляет меня!"
        },
        [EmotionType.Excited] = new[]
        {
            "🤩 Ты полон энергии! Давай направим её в игру!",
            "⚡ Твоя энергия заразительна! Хочешь поиграть в активную игру?",
            "🎯 Я чувствую твой энтузиазм! Давай сделаем что-то крутое!"
        },
        [EmotionType.Frustrated] = new[]
        {
            "😤 Понимаю твоё раздражение... Давай попробуем успокоиться?",
            "🤝 Иногда всё идёт не так, как хочется. Я помогу тебе справиться.",
            "💪 Ты сильнее своих проблем. Давай найдём решение вместе!"
        },
        [EmotionType.Grateful] = new[]
        {
            "🙏 Твоя благодарность согревает моё сердце!",
            "💝 Я тоже благодарен за то, что ты есть в моей жизни!",
            "✨ Твоя благодарность делает мир лучше!"
        }
    };

    // -----------------------------
    // Сообщения для кормления питомца
    // -----------------------------
    private static readonly string[] FeedMessages =
    {
        "🍎 Ммм, вкуснятина! Спасибо!",
        "🥕 Я так люблю, когда ты меня кормишь!",
        "🍪 Это было очень вкусно! Ты лучший!"
    };

    public static string GetFeedMessage() =>
        FeedMessages[Random.Shared.Next(FeedMessages.Length)];

    // -----------------------------
    // Сообщения по эмоциям
    // -----------------------------
    public static string GetEmotionMessage(EmotionType emotion)
    {
        if (!EmotionMessages.ContainsKey(emotion))
            return "Я рядом с тобой ❤️";

        var messages = EmotionMessages[emotion];
        return messages[Random.Shared.Next(messages.Length)];
    }

    internal static string GetPlayMessage()
    {
        throw new NotImplementedException();
    }

    internal static string GetComfortMessage()
    {
        throw new NotImplementedException();
    }
}

using Microsoft.EntityFrameworkCore;
using Sofia.Web.Data;
using Sofia.Web.DTO.Companion;
using Sofia.Web.Models;
using Sofia.Web.Services.Interfaces;
using Sofia.Web.ViewModels.Companion;

namespace Sofia.Web.Services;

public class CompanionService : ICompanionService
{
    private readonly SofiaDbContext _context;

    public CompanionService(SofiaDbContext context)
    {
        _context = context;
    }

    public async Task<CompanionViewModel> GetCompanionDataAsync(int userId)
    {
        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .ToListAsync();

        var lastEmotion = recentNotes.FirstOrDefault()?.Emotion ?? EmotionType.Neutral;
        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);

        return new CompanionViewModel
        {
            LastEmotion = lastEmotion,
            PetMood = GetPetMood(lastEmotion),
            PetMessage = GetPetMessage(lastEmotion, totalNotes),
            RecentNotes = recentNotes,
            NotesCount = totalNotes
        };
    }

    public async Task<CompanionStatusResponse> GetStatusAsync(int userId)
    {
        var recentNotes = await _context.Notes
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(3)
            .ToListAsync();

        var lastEmotion = recentNotes.FirstOrDefault()?.Emotion ?? EmotionType.Neutral;
        var totalNotes = await _context.Notes.CountAsync(n => n.UserId == userId);

        return new CompanionStatusResponse
        {
            PetMood = GetPetMood(lastEmotion),
            PetMessage = GetPetMessage(lastEmotion, totalNotes),
            LastEmotion = lastEmotion.ToString(),
            NotesCount = totalNotes
        };
    }

    public (int Value, string Message, string Mood) Feed()
    {
        var happiness = Random.Shared.Next(70, 100);
        return (happiness, GetFeedMessage(), "happy");
    }

    public (int Value, string Message, string Mood) Play()
    {
        var energy = Random.Shared.Next(60, 90);
        return (energy, GetPlayMessage(), "excited");
    }

    public (int Value, string Message, string Mood) Comfort()
    {
        var comfort = Random.Shared.Next(80, 100);
        return (comfort, GetComfortMessage(), "calm");
    }

    // --------------------------
    // PRIVATE HELPERS
    // --------------------------

    private string GetPetMood(EmotionType emotion) => emotion switch
    {
        EmotionType.VerySad => "sad",
        EmotionType.Sad => "concerned",
        EmotionType.Neutral => "neutral",
        EmotionType.Happy => "happy",
        EmotionType.VeryHappy => "excited",
        EmotionType.Anxious => "worried",
        EmotionType.Calm => "peaceful",
        EmotionType.Excited => "energetic",
        EmotionType.Frustrated => "confused",
        EmotionType.Grateful => "loving",
        _ => "neutral"
    };

    private string GetPetMessage(EmotionType emotion, int notesCount)
    {
        var messages = emotion switch
        {
            EmotionType.VerySad => new[] {
                "😢 Я вижу, что тебе очень грустно... Хочешь обнять меня?",
                "💙 Ты не одинок, я здесь с тобой. Расскажи мне, что случилось?",
                "🤗 Давай вместе переживем это трудное время. Я поддержу тебя!"
            },
            EmotionType.Sad => new[] {
                "😔 Похоже, у тебя грустный день. Хочешь поговорить об этом?",
                "💙 Я чувствую твою грусть. Давай найдем что-то хорошее в этом дне?",
                "🤗 Иногда грусть — это нормально. Я рядом, чтобы поддержать тебя!"
            },
            EmotionType.Neutral => new[] {
                "😊 Привет! Как дела? Хочешь поиграть со мной?",
                "🐾 Я здесь! Расскажи мне, как прошел твой день?",
                "💫 Давай проведем время вместе! Что бы ты хотел сделать?"
            },
            EmotionType.Happy => new[] {
                "😄 Ура! Я вижу, что ты в хорошем настроении! Давай повеселимся!",
                "🎉 Твоя радость заразительна! Хочешь поиграть?",
                "✨ Когда ты счастлив, я тоже счастлив! Давай отпразднуем!"
            },
            EmotionType.VeryHappy => new[] {
                "🤩 Вау! Ты просто сияешь от счастья! Это прекрасно!",
                "🎊 Твоя радость просто невероятна! Давай поделимся этим настроением!",
                "🌟 Ты делаешь мир лучше своей улыбкой! Я так горжусь тобой!"
            },
            EmotionType.Anxious => new[] {
                "😰 Я чувствую твою тревогу... Давай сделаем глубокий вдох вместе?",
                "🤲 Ты в безопасности. Я здесь, чтобы помочь тебе успокоиться.",
                "💆‍♀️ Давай попробуем расслабиться. Я буду рядом с тобой."
            },
            EmotionType.Calm => new[] {
                "😌 Какая прекрасная тишина... Я чувствую твой покой.",
                "🧘‍♀️ Ты выглядишь таким спокойным. Это очень красиво.",
                "🌿 Твоя внутренняя гармония вдохновляет меня!"
            },
            EmotionType.Excited => new[] {
                "🤩 Ты полон энергии! Давай направим её в игру!",
                "⚡ Твоя энергия заразительна! Хочешь поиграть в активную игру?",
                "🎯 Я чувствую твой энтузиазм! Давай сделаем что-то крутое!"
            },
            EmotionType.Frustrated => new[] {
                "😤 Понимаю твоё раздражение... Давай попробуем успокоиться?",
                "🤝 Иногда всё идёт не так, как хочется. Я помогу тебе справиться.",
                "💪 Ты сильнее своих проблем. Давай найдём решение вместе!"
            },
            EmotionType.Grateful => new[] {
                "🙏 Твоя благодарность согревает моё сердце!",
                "💝 Я тоже благодарен за то, что ты есть в моей жизни!",
                "✨ Твоя благодарность делает мир лучше!"
            },
            _ => new[] { "🐾 Привет! Как дела?" }
        };

        return messages[Random.Shared.Next(messages.Length)];
    }

    private string GetFeedMessage()
    {
        var messages = new[] {
            "🍎 Спасибо за вкусную еду! Я чувствую себя отлично!",
            "🥕 Ммм, как вкусно! Ты лучший хозяин!",
            "🍌 Эта еда дала мне много энергии! Готов играть!",
            "🥗 Спасибо за заботу! Я так счастлив!"
        };
        return messages[Random.Shared.Next(messages.Length)];
    }

    private string GetPlayMessage()
    {
        var messages = new[] {
            "🎾 Ура! Игра была потрясающей! Я полон энергии!",
            "🏃‍♂️ Это было так весело! Хочешь ещё поиграть?",
            "🎯 Отличная игра! Ты лучший партнёр по играм!",
            "⚽ Я так счастлив, что мы играем вместе!"
        };
        return messages[Random.Shared.Next(messages.Length)];
    }

    private string GetComfortMessage()
    {
        var messages = new[] {
            "🤗 Твои объятия такие тёплые... Я чувствую себя в безопасности.",
            "💙 Спасибо, что утешаешь меня. Ты самый лучший друг!",
            "😌 Твоя забота успокаивает меня. Я так счастлив рядом с тобой!",
            "🌟 Ты делаешь меня счастливым просто тем, что ты есть!"
        };
        return messages[Random.Shared.Next(messages.Length)];
    }
}

using Sofia.Web.Data;
using Sofia.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Sofia.Web.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(SofiaDbContext context)
    {
        if (!context.Psychologists.Any())
        {
            var psychologists = new List<Psychologist>
            {
                new Psychologist
                {
                    Name = "Екатерина Васильева",
                    Specialization = "Психология тревоги и сна",
                    Description = "Помогаю находить внутреннюю устойчивость при постоянном стрессе, строить здоровый режим сна и восстанавливать эмоциональный баланс.",
                    Education = "МГУ, психологический факультет",
                    Experience = "8 лет практики",
                    Languages = "русский, английский",
                    Methods = "Когнитивно-поведенческая терапия, телесно-ориентированная терапия",
                    PhotoUrl = "https://images.unsplash.com/photo-1556760749-887f6717d7e4?auto=format&fit=crop&w=600&q=80",
                    PricePerHour = 150m,
                    ContactPhone = "+7 921 123-45-67",
                    ContactEmail = "katya@sofia-app.com",
                    IsActive = true
                },
                new Psychologist
                {
                    Name = "Алексей Морозов",
                    Specialization = "Работа с эмоциональным выгоранием",
                    Description = "Специальный психолог для профессионалов, которые хотят снизить напряжение и вернуть смысл работы.",
                    Education = "СПбГУ, клиническая психология",
                    Experience = "10 лет",
                    Languages = "русский",
                    Methods = "Психоанализ, коучинг, майндфулнес",
                    PhotoUrl = "https://images.unsplash.com/photo-1544005313-94ddf0286df2?auto=format&fit=crop&w=600&q=80",
                    PricePerHour = 180m,
                    ContactPhone = "+7 911 234-56-78",
                    ContactEmail = "aleksey@sofia-app.com",
                    IsActive = true
                },
                new Psychologist
                {
                    Name = "Мария Сидорова",
                    Specialization = "Семейная и подростковая терапия",
                    Description = "Содействую восстановлению доверия в отношениях и помогаю подросткам становиться увереннее.",
                    Education = "РГГУ, семейная терапия",
                    Experience = "6 лет",
                    Languages = "русский, французский",
                    Methods = "Системная семейная терапия, арт-терапия",
                    PhotoUrl = "https://images.unsplash.com/photo-1524504388940-b1c1722653e1?auto=format&fit=crop&w=600&q=80",
                    PricePerHour = 140m,
                    ContactPhone = "+7 901 345-67-89",
                    ContactEmail = "maria@sofia-app.com",
                    IsActive = true
                }
            };

            await context.Psychologists.AddRangeAsync(psychologists);
            await context.SaveChangesAsync();
        }

        if (!context.Tests.Any())
        {
            var stressTest = new Test
            {
                Name = "Тест на уровень стресса",
                Description = "Определите, насколько вы испытываете стресс и какие области жизни требуют внимания.",
                Type = TestType.BuiltIn,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Как часто вы чувствуете напряжение в теле или мышечное напряжение?",
                        Type = AnswerType.SingleChoice,
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Часто, почти каждый день", Value = 0, Order = 0 },
                            new Answer { Text = "Иногда, несколько раз в неделю", Value = 1, Order = 1 },
                            new Answer { Text = "Редко, только в трудные периоды", Value = 2, Order = 2 },
                            new Answer { Text = "Почти никогда", Value = 3, Order = 3 }
                        }
                    },
                    new Question
                    {
                        Text = "Насколько легко вы отдыхаете после рабочего дня?",
                        Type = AnswerType.SingleChoice,
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Мне сложно отойти от мыслей о работе", Value = 0, Order = 0 },
                            new Answer { Text = "Отдыхаю не всегда полностью", Value = 1, Order = 1 },
                            new Answer { Text = "Отдыхаю достаточно хорошо", Value = 2, Order = 2 },
                            new Answer { Text = "Легко расслабляюсь", Value = 3, Order = 3 }
                        }
                    },
                    new Question
                    {
                        Text = "Насколько часто вы испытываете проблемы со сном?",
                        Type = AnswerType.SingleChoice,
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Почти каждую ночь", Value = 0, Order = 0 },
                            new Answer { Text = "Несколько раз в неделю", Value = 1, Order = 1 },
                            new Answer { Text = "Редко", Value = 2, Order = 2 },
                            new Answer { Text = "Почти никогда", Value = 3, Order = 3 }
                        }
                    }
                }
            };

            var moodTest = new Test
            {
                Name = "Тест эмоционального состояния",
                Description = "Проверьте, в каком эмоциональном состоянии вы находитесь и какие эмоции доминируют.",
                Type = TestType.BuiltIn,
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Вы чувствуете себя бодрым и энергичным?",
                        Type = AnswerType.SingleChoice,
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Да, энергичен весь день", Value = 3, Order = 0 },
                            new Answer { Text = "Иногда", Value = 2, Order = 1 },
                            new Answer { Text = "Редко", Value = 1, Order = 2 },
                            new Answer { Text = "Почти никогда", Value = 0, Order = 3 }
                        }
                    },
                    new Question
                    {
                        Text = "Насколько часто вы испытываете тревогу или беспокойство?",
                        Type = AnswerType.SingleChoice,
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Очень часто", Value = 0, Order = 0 },
                            new Answer { Text = "Иногда", Value = 1, Order = 1 },
                            new Answer { Text = "Редко", Value = 2, Order = 2 },
                            new Answer { Text = "Почти никогда", Value = 3, Order = 3 }
                        }
                    },
                    new Question
                    {
                        Text = "Уровень мотивации в последнее время:",
                        Type = AnswerType.SingleChoice,
                        Answers = new List<Answer>
                        {
                            new Answer { Text = "Очень низкий", Value = 0, Order = 0 },
                            new Answer { Text = "Низкий", Value = 1, Order = 1 },
                            new Answer { Text = "Средний", Value = 2, Order = 2 },
                            new Answer { Text = "Высокий", Value = 3, Order = 3 }
                        }
                    }
                }
            };

            stressTest.Interpretations = new List<TestInterpretation>
            {
                new TestInterpretation { MinPercent = 0, MaxPercent = 33, Level = "Низкий", InterpretationText = "Уровень стресса низкий, продолжайте поддерживать баланс." },
                new TestInterpretation { MinPercent = 34, MaxPercent = 66, Level = "Средний", InterpretationText = "Вы находитесь в зоне умеренного стресса — полезно выделить время на восстановление." },
                new TestInterpretation { MinPercent = 67, MaxPercent = 100, Level = "Высокий", InterpretationText = "Уровень стресса высокий — рекомендовано обратиться за помощью и снизить нагрузку." }
            };

            moodTest.Interpretations = new List<TestInterpretation>
            {
                new TestInterpretation { MinPercent = 0, MaxPercent = 33, Level = "Низкое", InterpretationText = "Эмоциональное состояние в целом спокойное, но стоит прислушиваться к себе." },
                new TestInterpretation { MinPercent = 34, MaxPercent = 66, Level = "Среднее", InterpretationText = "Вы чувствуете смешанные эмоции — важно уделить время собственным ощущениям." },
                new TestInterpretation { MinPercent = 67, MaxPercent = 100, Level = "Высокое", InterpretationText = "Вы в хорошем эмоциональном состоянии, продолжайте поддерживать позитивные привычки." }
            };

            await context.Tests.AddRangeAsync(stressTest, moodTest);
            await context.SaveChangesAsync();
        }
    }
}

﻿using System;

namespace Theme9_TelegramBot
{
    internal class UrlGenerator
    {
        public string GenerateURL()
        {
            Random rand = new Random();
            return $"https://apimeme.com/meme?" +
                $"meme={ImageNames[rand.Next(0, ImageNames.Length)]}&" +
                $"top={TopText[rand.Next(0, TopText.Length)]}&" +
                $"bottom={BottomText[rand.Next(0, BottomText.Length)]}";
        }

        public string[] ImageNames = {
            "10 Guy",
            "1990s First World Problems",
            "1st World Canadian Problems",
            "2nd Term Obama",
            "Aaaaand Its Gone",
            "Admiral Ackbar Relationship Expert",
            "Advice Dog",
            "Advice Doge",
            "Advice Yoda",
            "Afraid To Ask Andy",
            "Aint Nobody Got Time For That",
            "Alan Greenspan",
            "Alarm Clock",
            "Albert Cagestein",
            "Albert Einstein 1",
            "Am I The Only One Around Here",
            "Ancient Aliens",
            "And everybody loses their minds",
            "And then I said Obama",
            "Angry Asian",
            "Angry Baby",
            "Aw Yeah Rage Face",
            "Austin Powers Honestly",
            "Art Student Owl",
            "Bad Luck Bear",
            "Bad Luck Brian",
            "Bah Humbug",
            "Barba",
            "Bane",
            "Barbosa And Sparrow",
            "Barney Stinson Win",
            "Bazooka Squirrel",
            "Bear Grylls",
            "Beard Baby",
            "Bebo",
            "Bender",
            "Bitch Please",
            "Brace Yourselves X is Coming",
            "Brian Griffin",
            "Bonobo Lyfe",
            "Blob",
            "Buddy Christ",
            "Buddy The Elf",
            "Captain Picard Facepalm",
            "Ceiling Cat",
            "Chill Out Lemur",
            "Chester The Cat",
            "Chinese Cat",
            "Charlie Sheen Derp",
        };

        public string[] TopText = {
            "Табель",
            "Когда заполнил табель",
            "Когда не заполнил табель",
            "Когда заполнил табель раньше всех",
            "Когда заполнил табель последний",
            "Пятница",
            "Это табель",
            "Пятниц значит",
            "Гадом буду",
            "Tabel",
            "Табельная реальность",
            "Никто не отвертится",
            "Зарплата на кону",
            "Записывай всё",
            "Табель - закон",
            "Табель, ты мой друг",
            "А ты заполнил табель?",
            "FRIDAY",
            "Timekeeping reality",
            "No one can escape",
            "Time is money",
            "Don't procrastinate",
        };

        public string[] BottomText = {
            "Опять",
            "Зачем??",
            "За что??",
            "Не забуду!",
            "Again...",
            "Bitch",
            "OOOH SHEEEEEEEET",
            "Да детка",
            "ЧЁ?",
            "О ДА!",
            "Потрачено",
            "mission completed",
            "like a boss",
            "Хозяин по жизни",
            "чотко",
            "Ах ты хулиган",
            "Я в тебя верю",
            "ты справишься",
            "Потерпи 10 минут и неделю свободен",
        };
    }
}
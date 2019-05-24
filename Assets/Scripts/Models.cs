using System;

namespace PomoTimerApp
{
    public class Task
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        public string Title { get; set; }
        
        public string Description { get; set; }

        public int PomoCount { get; set; } = 0;

        public bool Done { get; set; } = false;
    }
}
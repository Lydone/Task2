namespace Task2.Models
{
    /// <summary>
    /// Заметка.
    /// </summary>
    public class Note
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Основной текст.
        /// </summary>
        public string Content { get; set; }
    }

    /// <summary>
    /// DTO-версия заметки.
    /// </summary>
    public class NoteDto {
        /// <summary>
        /// Заголовок.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Основной текст.
        /// </summary>
        public string Content { get; set; }
    }
}
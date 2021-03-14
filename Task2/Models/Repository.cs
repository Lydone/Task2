using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Task2.Models
{
    /// <summary>
    /// Репозиторий для хранения заметок.
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// Hashmap для хранения заметок.
        /// </summary>
        private readonly Dictionary<int, Note> dict;
        /// <summary>
        /// Счетчик заметок.
        /// </summary>
        private int _counter; 
        
        public Repository()
        {
            dict = new Dictionary<int, Note>();
            _counter = 0;
        }

        public Task<List<Note>> GetNotesAsync() {
            return Task.FromResult(dict.Values.ToList());
        }

        public Task<Note> GetNoteByIdAsync(int id)
        {
            if (dict.TryGetValue(id, out var result))
            {
                return Task.FromResult(result);
            }

            throw new DataException("Failed to retrieve exception");
        }

        public Task<Note> CreateNoteAsync(Note note)
        {
            note.Id = _counter;
            _counter += 1;
            dict.Add(note.Id, note);
            return Task.FromResult(note);
        }
        
        public async Task<Note> UpdateNoteAsync(int id, NoteDto dto)
        {
            var memo = await GetNoteByIdAsync(id);

            if (memo == null)
            {
                throw new DataException("No such note");
            }
            memo.Content = dto.Content;
            memo.Title = dto.Title;

            dict[memo.Id] = memo;

            return dict[memo.Id];
        }

        public Task<List<Note>> SearchAsync(string search)
        {
            var result = new List<Note>();
            
            foreach (var key in dict.Keys)
            {
                if (dict.TryGetValue(key, out var note))
                {
                    if (note.Content.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || note.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(note);
                    }
                }
            }
            
            return Task.FromResult(result);
        }

        public Task<bool> DeleteByIdAsync(int id)
        {
            if (dict.Remove(id, out var _))
            {
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
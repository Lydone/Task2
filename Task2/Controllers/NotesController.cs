using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Task2.Models;

namespace Task2.Controllers {
    /// <summary>
    /// Главный контроллер.
    /// </summary>
    [Produces("application/json")]
    [Route("notes")]
    public class MainController : Controller {
        private AdditionalOptions _options;
        private Repository _repository;

        public MainController(Repository repository, IOptions<AdditionalOptions> opt) {
            _options = opt.Value;
            _repository = repository;
        }

        /// <summary>
        /// Получить все заметки.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<Note>), 200)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetNotes([FromQuery] string searchString) {
            try {
                List<Note> notes;
                if (string.IsNullOrEmpty(searchString)) {
                    notes = await _repository.GetNotesAsync();
                } else {
                    notes = await _repository.SearchAsync(searchString);
                }
                if (notes.Count == 0) {
                    return StatusCode(StatusCodes.Status204NoContent, null);
                }

                if (HttpContext.Request.Headers.ContainsKey(_options.Header)) {
                    return Ok(notes);
                } else {
                    var note = notes.First();
                    var contentLength = Math.Min(_options.Num, note.Content.Length);
                    return Ok(note.Content.Substring(0, contentLength));
                }
            } catch (ArgumentException exc) {
                return BadRequest(exc.Message);
            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error");
            }
        }

        /// <summary>
        /// Получить заметку по Id.
        /// </summary>
        /// <param name="id">Id заметки.</param>
        /// <returns>Соответствующая заметка.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(List<Note>), 200)]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(string), 204)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> GetNoteById([FromRoute] int id) {
            try {
                var note = await _repository.GetNoteByIdAsync(id);
                if (note == null) {
                    return StatusCode(StatusCodes.Status204NoContent, null);
                }

                if (HttpContext.Request.Headers.ContainsKey(_options.Header)) {
                    return Ok(note);
                } else {
                    var contentLength = Math.Min(_options.Num, note.Content.Length);
                    return Ok(note.Content.Substring(0, contentLength));
                }
            } catch (DataException) {
                return StatusCode(StatusCodes.Status204NoContent, null);
            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something has gone wrong");
            }
        }

        /// <summary>
        /// Создать заметку.
        /// </summary>
        /// <param name="noteDto">Body запроса.</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(Note), 201)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> CreateNewNote([FromBody] NoteDto noteDto) {
            try {
                var newNote = await _repository.CreateNoteAsync(new Note {
                    Title = noteDto.Title,
                    Content = noteDto.Content
                });

                return StatusCode(StatusCodes.Status201Created, newNote);
            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something has gone wrong");
            }
        }

        /// <summary>
        /// Редактировать заметку.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <param name="noteDto">Новая заметка.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Note), 200)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> EditNote([FromRoute] int id, [FromBody] NoteDto noteDto) {
            try {
                var newMemo = await _repository.UpdateNoteAsync(id, noteDto);

                return Ok(newMemo);
            } catch (DataException exc) {
                return BadRequest(exc.Message);
            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something has gone wrong");
            }
        }

        /// <summary>
        /// Удалить заметку по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> DeleteNote([FromRoute] int id) {
            try {
                var result = await _repository.DeleteByIdAsync(id);

                return Ok(result);
            } catch (Exception) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Something has gone wrong");
            }
        }
    }
}
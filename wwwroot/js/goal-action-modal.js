/**
 * GoalAction Modal Management
 * Управление модалями для целей
 */

class GoalActionModal {
  constructor() {
    this.actionModalOverlay = document.getElementById('actionModalOverlay');
    this.actionModal = document.getElementById('actionModal');
    this.actionForm = document.getElementById('actionForm');
    this.actionCloseBtn = document.getElementById('actionCloseBtn');
    this.actionSaveBtn = document.getElementById('actionSaveBtn');
    this.currentGoalId = null;

    this.init();
  }

  init() {
    if (this.actionCloseBtn) {
      this.actionCloseBtn.addEventListener('click', () => this.closeModal());
    }

    if (this.actionModalOverlay) {
      this.actionModalOverlay.addEventListener('click', (e) => {
        if (e.target === this.actionModalOverlay) {
          this.closeModal();
        }
      });
    }

    if (this.actionSaveBtn) {
      this.actionSaveBtn.addEventListener('click', () => this.saveAction());
    }

    // Обработчики для кнопок действий
    this.attachActionButtonListeners();
  }

  attachActionButtonListeners() {
    document.addEventListener('click', (e) => {
      if (e.target.classList.contains('btn-goal-action')) {
        const goalId = e.target.closest('.goal-card')?.dataset.goalId || 
                      e.target.dataset.goalId;
        if (goalId) {
          this.openModal(parseInt(goalId));
        }
      }

      if (e.target.classList.contains('btn-goal-history')) {
        const goalId = e.target.closest('.goal-card')?.dataset.goalId || 
                      e.target.dataset.goalId;
        if (goalId) {
          this.openHistoryModal(parseInt(goalId));
        }
      }

      if (e.target.classList.contains('btn-delete-action')) {
        const actionId = e.target.dataset.actionId;
        if (actionId && confirm('Удалить это действие?')) {
          this.deleteAction(parseInt(actionId));
        }
      }
    });
  }

  openModal(goalId) {
    this.currentGoalId = goalId;
    
    // Очистить форму
    if (this.actionForm) {
      this.actionForm.reset();
    }

    // Показать модаль
    if (this.actionModalOverlay) {
      this.actionModalOverlay.classList.add('active');
    }
  }

  closeModal() {
    if (this.actionModalOverlay) {
      this.actionModalOverlay.classList.remove('active');
    }
    this.currentGoalId = null;
  }

  async saveAction() {
    if (!this.currentGoalId) {
      alert('Ошибка: ID цели не найден');
      return;
    }

    const actionText = document.getElementById('actionText')?.value?.trim();
    const resultText = document.getElementById('resultText')?.value?.trim();

    if (!actionText || !resultText) {
      alert('Пожалуйста, заполните все поля');
      return;
    }

    try {
      const response = await fetch('/api/goals/add-action', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-CSRF-Token': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
        },
        body: JSON.stringify({
          goalId: this.currentGoalId,
          actionText,
          resultText
        })
      });

      if (response.ok) {
        alert('Действие сохранено!');
        this.closeModal();
        
        // Обновить страницу или историю
        location.reload();
      } else {
        const error = await response.json();
        alert('Ошибка: ' + (error.message || 'Не удалось сохранить действие'));
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Произошла ошибка при сохранении');
    }
  }

  async openHistoryModal(goalId) {
    try {
      const response = await fetch(`/api/goals/${goalId}/actions`);
      const actions = await response.json();

      const historyForm = document.getElementById('historyForm');
      if (historyForm) {
        let html = '<div class="action-history">';

        if (actions.length === 0) {
          html += `
            <div class="empty-state">
              <div class="empty-state-icon">📝</div>
              <div class="empty-state-text">Нет записанных действий</div>
            </div>
          `;
        } else {
          actions.forEach(action => {
            const date = new Date(action.createdAt).toLocaleString('ru-RU');
            html += `
              <div class="action-item">
                <div class="action-item-date">${date}</div>
                <div class="action-item-action">
                  <strong>✓ Что сделано:</strong>
                  ${this.escapeHtml(action.actionText)}
                </div>
                <div class="action-item-result">
                  <strong>📊 Результат:</strong>
                  ${this.escapeHtml(action.resultText)}
                </div>
                <button type="button" class="action-item-delete btn-delete-action" data-action-id="${action.id}">
                  Удалить
                </button>
              </div>
            `;
          });
        }

        html += '</div>';
        historyForm.innerHTML = html;

        // Показать модаль истории
        const historyModalOverlay = document.getElementById('historyModalOverlay');
        if (historyModalOverlay) {
          historyModalOverlay.classList.add('active');
        }

        // Переприсоединить слушатели для новых кнопок удаления
        this.attachActionButtonListeners();
      }
    } catch (error) {
      console.error('Error loading history:', error);
      alert('Ошибка при загрузке истории');
    }
  }

  async deleteAction(actionId) {
    try {
      const response = await fetch(`/api/goals/delete-action/${actionId}`, {
        method: 'DELETE',
        headers: {
          'X-CSRF-Token': document.querySelector('[name="__RequestVerificationToken"]')?.value || ''
        }
      });

      if (response.ok) {
        alert('Действие удалено');
        location.reload();
      } else {
        alert('Ошибка при удалении');
      }
    } catch (error) {
      console.error('Error:', error);
      alert('Произошла ошибка');
    }
  }

  escapeHtml(text) {
    const map = {
      '&': '&amp;',
      '<': '&lt;',
      '>': '&gt;',
      '"': '&quot;',
      "'": '&#039;'
    };
    return text.replace(/[&<>"']/g, m => map[m]);
  }
}

// Инициализировать модали при загрузке страницы
document.addEventListener('DOMContentLoaded', () => {
  new GoalActionModal();
});

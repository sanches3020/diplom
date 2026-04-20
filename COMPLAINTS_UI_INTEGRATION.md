# Интеграция жалоб в UI - Контекстное меню

## Обзор

Эта документация описывает, как интегрировать функцию жалоб в контекстное меню различных компонентов приложения.

## 1. Жалоба на сообщение в чате

### Расположение меню
Кнопка "⋮" в правом углу сообщения → "Пожаловаться"

### Implementation в компоненте ChatMessage

```html
<!-- В компоненте сообщения чата -->
<div class="message-item">
  <div class="message-header">
    <span class="message-author">@message.UserName</span>
    <div class="message-actions dropdown">
      <button class="btn btn-sm btn-link dropdown-toggle" type="button" data-bs-toggle="dropdown">
        ⋮
      </button>
      <ul class="dropdown-menu">
        <li>
          <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#complaintModal" 
             data-message-id="@message.Id"
             data-user-id="@message.UserId">
            🚩 Пожаловаться
          </a>
        </li>
      </ul>
    </div>
  </div>
  <div class="message-content">
    @message.Text
  </div>
</div>
```

### JavaScript для обработки

```javascript
// Инициализация модального окна жалобы
document.addEventListener('DOMContentLoaded', function() {
    const complaintModal = document.getElementById('complaintModal');
    
    if (complaintModal) {
        complaintModal.addEventListener('show.bs.modal', function(event) {
            const button = event.relatedTarget;
            const messageId = button.getAttribute('data-message-id');
            const targetUserId = button.getAttribute('data-user-id');
            
            // Установить значения в форму
            document.getElementById('complaintTargetUserId').value = targetUserId;
            document.getElementById('complaintMessageId').value = messageId;
            document.getElementById('complaintType').dataset.type = 'message';
        });
    }
});

// Функция отправки жалобы
async function submitComplaint() {
    const targetUserId = document.getElementById('complaintTargetUserId').value;
    const messageId = document.getElementById('complaintMessageId').value;
    const reason = parseInt(document.getElementById('complaintReason').value);
    const reasonText = document.getElementById('complaintReasonText').value;
    
    try {
        const response = await fetch('/api/complaints', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': document.querySelector('[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({
                targetUserId: targetUserId,
                messageId: messageId,
                postId: null,
                reason: reason,
                reasonText: reasonText
            })
        });
        
        if (response.ok) {
            alert('Жалоба успешно отправлена администраторам');
            bootstrap.Modal.getInstance(document.getElementById('complaintModal')).hide();
        } else {
            const data = await response.json();
            alert('Ошибка: ' + (data.message || 'Не удалось отправить жалобу'));
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ошибка при отправке жалобы');
    }
}
```

## 2. Жалоба на пост в форуме

### Расположение меню
Кнопка "⋮" на посте → "Пожаловаться"

### Implementation в компоненте ForumPost

```html
<!-- В компоненте поста форума -->
<div class="forum-post card mb-3">
  <div class="card-header d-flex justify-content-between align-items-center">
    <span>@post.Author.FullName</span>
    <div class="dropdown">
      <button class="btn btn-sm btn-link dropdown-toggle" type="button" data-bs-toggle="dropdown">
        ⋮
      </button>
      <ul class="dropdown-menu">
        <li>
          <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#complaintModal"
             data-post-id="@post.Id"
             data-user-id="@post.AuthorId">
            🚩 Пожаловаться на пост
          </a>
        </li>
      </ul>
    </div>
  </div>
  <div class="card-body">
    @post.Content
  </div>
</div>
```

### JavaScript для обработки

```javascript
// Модификация обработчика для поддержки постов
document.addEventListener('DOMContentLoaded', function() {
    const complaintModal = document.getElementById('complaintModal');
    
    if (complaintModal) {
        complaintModal.addEventListener('show.bs.modal', function(event) {
            const button = event.relatedTarget;
            const postId = button.getAttribute('data-post-id');
            const messageId = button.getAttribute('data-message-id');
            const targetUserId = button.getAttribute('data-user-id');
            
            // Очистить значения
            document.getElementById('complaintMessageId').value = messageId || '';
            document.getElementById('complaintPostId').value = postId || '';
            document.getElementById('complaintTargetUserId').value = targetUserId;
            
            // Обновить тип
            if (postId) {
                document.getElementById('complaintType').dataset.type = 'post';
            } else {
                document.getElementById('complaintType').dataset.type = 'message';
            }
        });
    }
});
```

## 3. Жалоба на профиль пользователя

### Расположение меню
Кнопка "⋮" на профиле пользователя → "Пожаловаться"

### Implementation на странице профиля

```html
<!-- На странице профиля пользователя -->
<div class="profile-header">
  <div class="profile-actions">
    <div class="dropdown">
      <button class="btn btn-link dropdown-toggle" type="button" data-bs-toggle="dropdown">
        ⋮
      </button>
      <ul class="dropdown-menu">
        <li>
          <a class="dropdown-item text-danger" href="#" data-bs-toggle="modal" 
             data-bs-target="#complaintModal"
             data-user-id="@model.Id">
            🚩 Пожаловаться на пользователя
          </a>
        </li>
      </ul>
    </div>
  </div>
</div>
```

## 4. Модальное окно жалобы (Universal)

### HTML Modal

```html
<!-- Универсальное модальное окно для жалоб -->
<div class="modal fade" id="complaintModal" tabindex="-1">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Пожаловаться</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
      </div>
      <div class="modal-body">
        <form id="complaintForm">
          <!-- Скрытые поля -->
          <input type="hidden" id="complaintType" />
          <input type="hidden" id="complaintTargetUserId" />
          <input type="hidden" id="complaintMessageId" />
          <input type="hidden" id="complaintPostId" />
          
          <!-- Видимые поля -->
          <div class="mb-3">
            <label for="complaintReason" class="form-label">Причина жалобы *</label>
            <select id="complaintReason" class="form-select" required>
              <option value="">-- Выберите причину --</option>
              <option value="0">Преследование / Оскорбление</option>
              <option value="1">Спам</option>
              <option value="2">Взрослый контент</option>
              <option value="3">Вредоносный контент</option>
              <option value="4">Медицинская дезинформация</option>
              <option value="5">Выдача себя за другого</option>
              <option value="6">Нарушение приватности</option>
              <option value="7">Нарушение авторских прав</option>
              <option value="8">Другое</option>
            </select>
          </div>
          
          <div class="mb-3">
            <label for="complaintReasonText" class="form-label">Дополнительная информация</label>
            <textarea id="complaintReasonText" class="form-control" rows="4" 
                      placeholder="Опишите, почему вы подаете жалобу (опционально)"></textarea>
            <small class="text-muted">Максимум 1000 символов</small>
          </div>
          
          <div class="alert alert-info" role="alert">
            <strong>Важно:</strong> Жалобы будут рассмотрены администраторами в течение 24 часов.
            Ложные жалобы могут привести к ограничениям на вашем аккаунте.
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
        <button type="button" class="btn btn-primary" onclick="submitComplaint()">Отправить жалобу</button>
      </div>
    </div>
  </div>
</div>

<!-- Anti-forgery token (должен быть на странице) -->
<input type="hidden" name="__RequestVerificationToken" />
```

## 5. Обработка ошибок и UX

### Проверка перед отправкой

```javascript
function validateComplaint() {
    const targetUserId = document.getElementById('complaintTargetUserId').value;
    const reason = document.getElementById('complaintReason').value;
    
    if (!targetUserId) {
        alert('Ошибка: Не удалось определить объект жалобы');
        return false;
    }
    
    if (!reason) {
        alert('Пожалуйста, выберите причину жалобы');
        return false;
    }
    
    return true;
}

async function submitComplaint() {
    if (!validateComplaint()) return;
    
    const submitButton = event.target;
    submitButton.disabled = true;
    submitButton.textContent = 'Отправка...';
    
    try {
        const response = await fetch('/api/complaints', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': document.querySelector('[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify({
                targetUserId: document.getElementById('complaintTargetUserId').value,
                messageId: document.getElementById('complaintMessageId').value || null,
                postId: document.getElementById('complaintPostId').value ? 
                       parseInt(document.getElementById('complaintPostId').value) : null,
                reason: parseInt(document.getElementById('complaintReason').value),
                reasonText: document.getElementById('complaintReasonText').value
            })
        });
        
        if (response.status === 201) {
            alert('Спасибо! Жалоба успешно отправлена администраторам.');
            document.getElementById('complaintForm').reset();
            bootstrap.Modal.getInstance(document.getElementById('complaintModal')).hide();
        } else if (response.status === 400) {
            const data = await response.json();
            alert('Ошибка: ' + (data.message || 'Не удалось отправить жалобу'));
        } else if (response.status === 401) {
            alert('Пожалуйста, войдите в свой аккаунт');
        } else {
            alert('Неожиданная ошибка. Пожалуйста, попробуйте позже.');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ошибка сети. Пожалуйста, проверьте подключение.');
    } finally {
        submitButton.disabled = false;
        submitButton.textContent = 'Отправить жалобу';
    }
}
```

## 6. Стили CSS

```css
/* Кнопка контекстного меню */
.message-actions .dropdown-toggle,
.post-actions .dropdown-toggle,
.profile-actions .dropdown-toggle {
    color: #999;
    text-decoration: none;
}

.message-actions .dropdown-toggle:hover,
.post-actions .dropdown-toggle:hover,
.profile-actions .dropdown-toggle:hover {
    color: #666;
}

/* Пункт меню "Пожаловаться" */
.dropdown-menu .dropdown-item.text-danger:hover {
    background-color: #ffe5e5;
}

/* Модальное окно */
#complaintModal .modal-content {
    border-radius: 0.5rem;
}

#complaintModal .form-select,
#complaintModal .form-control {
    border-color: #dee2e6;
}

#complaintModal .form-select:focus,
#complaintModal .form-control:focus {
    border-color: #007bff;
    box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}
```

## 7. Интеграция с SignalR (для реал-тайм обновлений - опционально)

```javascript
// Если используется SignalR для чата
if (window.chatConnection) {
    chatConnection.on("ComplaintSubmitted", function(complaintId) {
        // Можно обновить UI, если нужно
        console.log("New complaint submitted: " + complaintId);
    });
}
```

## 8. Ограничения и ограждения

### Клиентские проверки

- Максимум 5 жалоб в час на пользователя
- Нельзя жаловаться на себя
- Нельзя подавать одну и ту же жалобу дважды

### Сообщения об ошибках

```javascript
const complaintErrors = {
    'Cannot complain about yourself': 'Вы не можете жаловаться на себя',
    'You have already complained about this': 'Вы уже подали жалобу на этот контент',
    'Message not found': 'Сообщение не найдено или было удалено',
    'Post not found': 'Пост не найден или был удален',
    'Target user not found': 'Пользователь не найден'
};
```

## 9. Testing Checklist

- [ ] Жалоба на сообщение чата
- [ ] Жалоба на пост форума  
- [ ] Жалоба на профиль пользователя
- [ ] Нельзя пожаловаться на себя
- [ ] Нельзя подать одну жалобу дважды
- [ ] Модальное окно очищается после отправки
- [ ] Выводятся корректные сообщения об ошибках
- [ ] Администратор видит жалобу в панели
- [ ] Уведомление отправляется администраторам
- [ ] Работает применение санкций (бан/предупреждение)

## После entwicklung TODO

1. [ ] Добавить компонент жалоб в компоненты чата и форума
2. [ ] Интегрировать модальное окно в основной layout
3. [ ] Протестировать все сценарии
4. [ ] Добавить аналитику отправленных жалоб
5. [ ] Добавить информационное сообщение при первой жалобе

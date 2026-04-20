# Инструкции по развертыванию системы жалоб

## 1. Применение миграции БД

```bash
# Убедитесь, что вы в папке проекта
cd /workspaces/diplom

# Применить миграцию к БД
dotnet-ef database update --project Sofia.Web.csproj
```

После успешного выполнения команды в БД будет создана таблица `Complaints` с необходимыми индексами.

## 2. Построение проекта

```bash
# Построить проект в режиме Debug
dotnet build Sofia.Web.csproj

# Или построить в режиме Release (для production)
dotnet build Sofia.Web.csproj -c Release
```

Проект должен собраться без ошибок (могут быть предупреждения, которые не критичны).

## 3. Запуск приложения

```bash
# Запустить приложение
dotnet run --project Sofia.Web.csproj

# Или для production окружения
dotnet run --project Sofia.Web.csproj -c Release --no-build
```

Приложение будет доступно по адресу: `https://localhost:5001` или `https://localhost:7135`

## 4. Проверка работы

### Проверка в браузере

1. **Администратор панель жалоб**
   - Перейти на `https://localhost:5001/admin/complaints`
   - Должны увидеть пустой список жалоб (если еще нет жалоб)

2. **Статистика**
   - В верхней части должны быть виджеты со счетчиками

3. **Фильтры**
   - Должны работать фильтры по статусу

### Проверка API

```bash
# 1. Получить все жалобы (если есть)
curl -H "Authorization: Bearer {admin-token}" \
  https://localhost:5001/api/complaints/admin/all

# 2. Получить статистику
curl -H "Authorization: Bearer {admin-token}" \
  https://localhost:5001/api/complaints/admin/stats

# 3. Получить счетчик новых жалоб
curl -H "Authorization: Bearer {admin-token}" \
  https://localhost:5001/api/complaints/admin/unreviewed-count
```

## 5. Интеграция в UI (Следующий этап)

После проверки работы системы жалоб на backend, нужно интегрировать UI компоненты:

### Чат
Добавить контекстное меню "⋮" в компоненты сообщений:

```html
<!-- В ChatMessage компоненте -->
<div class="message-actions">
  <button class="dropdown-toggle" data-bs-toggle="dropdown">⋮</button>
  <ul class="dropdown-menu">
    <li>
      <a class="dropdown-item" href="#" 
         data-bs-toggle="modal" 
         data-bs-target="#complaintModal"
         data-message-id="@message.Id"
         data-user-id="@message.UserId">
        🚩 Пожаловаться
      </a>
    </li>
  </ul>
</div>
```

### Форум
Добавить контекстное меню на посты:

```html
<!-- В ForumPost компоненте -->
<div class="post-actions">
  <button class="dropdown-toggle" data-bs-toggle="dropdown">⋮</button>
  <ul class="dropdown-menu">
    <li>
      <a class="dropdown-item" href="#"
         data-bs-toggle="modal"
         data-bs-target="#complaintModal"
         data-post-id="@post.Id"
         data-user-id="@post.AuthorId">
        🚩 Пожаловаться на пост
      </a>
    </li>
  </ul>
</div>
```

### Профили
Добавить контекстное меню на профиль:

```html
<!-- На странице профиля -->
<div class="profile-actions">
  <button class="dropdown-toggle" data-bs-toggle="dropdown">⋮</button>
  <ul class="dropdown-menu">
    <li>
      <a class="dropdown-item" href="#"
         data-bs-toggle="modal"
         data-bs-target="#complaintModal"
         data-user-id="@model.Id">
        🚩 Пожаловаться на пользователя
      </a>
    </li>
  </ul>
</div>
```

### Модальное окно

Добавить универсальное модальное окно жалобы на главную страницу (в Shared/Layout):

```html
<!-- Модальное окно жалобы -->
<div class="modal fade" id="complaintModal" tabindex="-1">
  <div class="modal-dialog">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title">Пожаловаться</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
      </div>
      <div class="modal-body">
        <form id="complaintForm">
          <input type="hidden" id="complaintTargetUserId" />
          <input type="hidden" id="complaintMessageId" />
          <input type="hidden" id="complaintPostId" />
          
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
            <textarea id="complaintReasonText" class="form-control" rows="4"></textarea>
          </div>
        </form>
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Отмена</button>
        <button type="button" class="btn btn-primary" onclick="submitComplaint()">Отправить</button>
      </div>
    </div>
  </div>
</div>
```

JavaScript для обработки:

```javascript
async function submitComplaint() {
    const form = document.getElementById('complaintForm');
    const data = {
        targetUserId: document.getElementById('complaintTargetUserId').value,
        messageId: document.getElementById('complaintMessageId').value || null,
        postId: document.getElementById('complaintPostId').value ? 
               parseInt(document.getElementById('complaintPostId').value) : null,
        reason: parseInt(document.getElementById('complaintReason').value),
        reasonText: document.getElementById('complaintReasonText').value
    };
    
    try {
        const response = await fetch('/api/complaints', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': document.querySelector('[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(data)
        });
        
        if (response.ok) {
            alert('Жалоба успешно отправлена администраторам');
            form.reset();
            bootstrap.Modal.getInstance(document.getElementById('complaintModal')).hide();
        } else {
            alert('Ошибка при отправке жалобы');
        }
    } catch (error) {
        console.error('Error:', error);
        alert('Ошибка при отправке жалобы');
    }
}
```

## 6. Тестовые сценарии

### Сценарий 1: Создание жалобы через API

```bash
curl -X POST https://localhost:5001/api/complaints \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {token}" \
  -d '{
    "targetUserId": "user-id",
    "messageId": "message-guid",
    "reason": 0,
    "reasonText": "Оскорбления"
  }'

# Ожидаемый результат: 201 Created с данными жалобы
```

### Сценарий 2: Просмотр жалоб администратором

```bash
curl -X GET https://localhost:5001/api/complaints/admin/all \
  -H "Authorization: Bearer {admin-token}"

# Ожидаемый результат: 200 OK с массивом жалоб
```

### Сценарий 3: Обновление статуса жалобы

```bash
curl -X PATCH https://localhost:5001/api/complaints/1/status \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {admin-token}" \
  -d '{
    "status": 2,
    "applyBan": true,
    "adminComment": "Заблокирован за оскорбления"
  }'

# Ожидаемый результат: 204 No Content
```

## 7. Логирование и мониторинг

### Проверка логов

Логи действий администраторов сохраняются в таблице `AdminLogs`:

```sql
SELECT * FROM "AspNetUserClaims" 
WHERE "ClaimType" = 'BlockUserFromComplaint'
ORDER BY "CreatedAt" DESC;
```

### Проверка уведомлений

Уведомления сохраняются в таблице `Notifications`:

```sql
SELECT * FROM "Notifications"
WHERE "Type" IN (20, 21)  -- ComplaintCreated, ComplaintUpdated
ORDER BY "CreatedAt" DESC;
```

## 8. Возможные проблемы и решения

### Проблема: Миграция не применяется

**Решение:**
```bash
# Проверить статус миграций
dotnet-ef database list-update-pending --project Sofia.Web.csproj

# Откатить и применить заново
dotnet-ef database rollback --project Sofia.Web.csproj
dotnet-ef database update --project Sofia.Web.csproj
```

### Проблема: 401 Unauthorized при обращении к API

**Решение:**
- Убедитесь, что используется правильный JWT токен
- Токен должен содержать правильное имя пользователя/ID
- Для администраторских операций нужна роль `admin`

### Проблема: 400 Bad Request при создании жалобы

**Решение:**
- Проверьте, что пользователь авторизован
- Проверьте, что не жалоуетесь на себя
- Проверьте, что не подали одну жалобу дважды
- Проверьте, не превышен ли лимит 5 жалоб в час

## 9. Production развертывание

Для развертывания в production:

1. Установите переменные окружения
2. Обновите строку подключения к БД
3. Отключите режим разработки
4. Включите HTTPS
5. Настройте логирование
6. Создайте резервные копии

```bash
# Build для production
dotnet publish Sofia.Web.csproj -c Release -o ./publish

# Запуск
cd publish
./Sofia.Web
```

## 10. Документация

Для получения более подробной информации смотрите:
- [COMPLAINTS_SYSTEM.md](COMPLAINTS_SYSTEM.md) - техническая документация
- [COMPLAINTS_UI_INTEGRATION.md](COMPLAINTS_UI_INTEGRATION.md) - руководство UI
- [IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md) - резюме реализации

---

**Версия**: 1.0  
**Последнее обновление**: 20.04.2026

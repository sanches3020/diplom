# Система жалоб и модерации контента

## Обзор

Реализована полнофункциональная система для подачи жалоб на контент в Sofia App. Система обеспечивает модерацию контента с сохранением приватности личных сообщений. Жалобы могут отправлять как обычные пользователи, так и психологи.

## Архитектура

### 1. Модели (Models)

#### `Complaint.cs`
Основная модель жалобы с полями:
- **Id** - уникальный идентификатор
- **SenderUserId** - ID пользователя, отправившего жалобу
- **TargetUserId** - ID пользователя, на которого жалоба (обязателен)
- **MessageId** - ID сообщения чата (nullable)
- **PostId** - ID поста форума (nullable)
- **Reason** - перечисление причин жалобы
- **ReasonText** - дополнительный текст причины
- **Status** - статус жалобы (New, Reviewed, Resolved, Dismissed)
- **ReviewedByAdminId** - ID администратора, рассматривавшего жалобу
- **ReviewedAt** - дата рассмотрения
- **AdminComment** - комментарий администратора
- **IsBanned** - был ли применен бан
- **IsWarned** - было ли выдано предупреждение
- **CreatedAt** / **UpdatedAt** - временные метки

#### Перечисления (Enums)

**ComplaintStatus:**
```csharp
public enum ComplaintStatus
{
    New = 0,          // Новая жалоба
    Reviewed = 1,     // Просмотрена администратором
    Resolved = 2,     // Разрешена (принято действие)
    Dismissed = 3     // Отклонена (нарушения не найдено)
}
```

**ComplaintReason:**
```csharp
public enum ComplaintReason
{
    Harassment = 0,           // Преследование/Оскорбление
    Spam = 1,                 // Спам
    AdultContent = 2,         // Взрослый контент
    HarmfulContent = 3,       // Вредоносный контент
    MedicalFalseInfo = 4,     // Медицинская дезинформация
    Impersonation = 5,        // Выдача себя за другого
    PrivacyViolation = 6,     // Нарушение приватности
    CopyrightViolation = 7,   // Нарушение авторских прав
    Other = 8                 // Другое
}
```

### 2. DTO (Data Transfer Objects)

#### `CreateComplaintRequest.cs`
Запрос для создания жалобы:
```csharp
public class CreateComplaintRequest
{
    public string TargetUserId { get; set; }      // Обязателен
    public string? MessageId { get; set; }         // Опционально
    public int? PostId { get; set; }              // Опционально
    public int Reason { get; set; }               // 0-8
    public string? ReasonText { get; set; }       // Опционально
}
```

#### `ComplaintDto.cs`
DTO для отправки данных о жалобе (с контентом для администратора):
- Содержит все поля жалобы
- Включает `MessageContent` и `PostContent` для администратора
- Отправляется только администраторам для просмотра

#### `UpdateComplaintStatusRequest.cs`
Запрос для обновления статуса жалобы:
```csharp
public class UpdateComplaintStatusRequest
{
    public int Status { get; set; }        // 0=New, 1=Reviewed, 2=Resolved, 3=Dismissed
    public string? AdminComment { get; set; }
    public bool ApplyBan { get; set; }
    public bool ApplyWarning { get; set; }
}
```

### 3. Сервисы (Services)

#### `IComplaintService` и `ComplaintService.cs`

**Основные методы:**

1. **CreateComplaintAsync**
   - Создает новую жалобу
   - Проверяет валидацию (не может жаловаться на себя, на одно и то же дважды)
   - Защита от спама (макс 5 жалоб в час)
   - Отправляет уведомление администраторам

2. **GetAllComplaintsAsync**
   - Получает все жалобы (только для администраторов)
   - Опциональный фильтр по статусу

3. **GetComplaintsOnUserAsync**
   - Получает жалобы на конкретного пользователя

4. **GetComplaintByIdAsync**
   - Получает деталь жалобы с контентом

5. **UpdateComplaintStatusAsync**
   - Обновляет статус жалобы
   - Применяет санкции (бан, предупреждение)
   - Логирует действие
   - Отправляет уведомления

6. **GetUnreviewedCountAsync**
   - Возвращает количество новых жалоб

7. **GetComplaintStatsAsync**
   - Статистика по жалобам (всего, по статусам, по пользователям)

8. **CanComplainAsync**
   - Проверяет возможность подачи жалобы
   - Валидирует существование объекта жалобы
   - Проверяет дубликаты

### 4. Контроллеры (Controllers)

#### `ComplaintsController.cs` - API endpoints

**User Endpoints:**
- `POST /api/complaints` - Подать жалобу (авторизован)

**Admin Endpoints:**
- `GET /api/complaints/{id}` - Получить жалобу
- `GET /api/complaints/admin/all` - Все жалобы
- `GET /api/complaints/admin/user/{targetUserId}` - На конкретного пользователя
- `PATCH /api/complaints/{id}/status` - Обновить статус
- `GET /api/complaints/admin/unreviewed-count` - Количество новых
- `GET /api/complaints/admin/stats` - Статистика

#### `AdminController.cs` - Web UI

**Complaint Management Pages:**
- `GET /admin/complaints` - Список всех жалоб с фильтрацией
- `GET /admin/complaints/{id}` - Детальный просмотр жалобы
- `POST /admin/complaints/{id}/status` - Обновление статуса
- `GET /admin/complaints/user/{userId}` - Жалобы на пользователя

## Использование

### Для пользователя: Отправить жалобу

**Пример API запроса:**

```javascript
// На сообщение в чате
POST /api/complaints
{
  "targetUserId": "user-id",
  "messageId": "message-guid",
  "reason": 0,  // Harassment
  "reasonText": "Пользователь меня оскорбляет"
}

// На пост форума
POST /api/complaints
{
  "targetUserId": "user-id",
  "postId": 123,
  "reason": 2,  // AdultContent
  "reasonText": "Неприемлемый контент"
}

// На пользователя (общая жалоба)
POST /api/complaints
{
  "targetUserId": "user-id",
  "reason": 8,  // Other
  "reasonText": "Описание нарушения"
}
```

**Важные особенности:**
- Максимум 5 жалоб от одного пользователя в час
- Нельзя жаловаться на себя
- Нельзя подавать две одинаковые жалобы
- При создании отправляется уведомление администраторам

### Для администратора: Управление жалобами

**Web Interface:**
1. Перейти на `/admin/complaints`
2. Просмотреть статистику в верхней части страницы
3. Отфильтровать жалобы по статусу
4. Нажать "Просмотреть" для деталей
5. На странице деталей:
   - Видеть весь контент, на который жалоба
   - Видеть данные пользователя, на которого жалоба
   - Видеть причину жалобы
   - Принять действие:
     - Отметить как рассмотрённую
     - Разрешить с применением санкций (бан/предупреждение)
     - Отклонить

**API для программного доступа:**

```javascript
// Получить все новые жалобы
GET /api/complaints/admin/all?status=0

// Рассмотреть жалобу
PATCH /api/complaints/1/status
{
  "status": 1  // Reviewed
}

// Разрешить жалобу и заблокировать пользователя
PATCH /api/complaints/1/status
{
  "status": 2,           // Resolved
  "applyBan": true,
  "adminComment": "Пользователь заблокирован за харассмент"
}
```

## Приватность и безопасность

### ✅ Соблюдение приватности

1. **Администратор НЕ видит:**
   - История чата между пользователями
   - Другие сообщения в одной комнате
   - Личные данные пользователей (кроме необходимых для жалобы)

2. **Администратор видит ТОЛЬКО:**
   - Конкретное сообщение, на которое пожаловались
   - Конкретный пост, на который пожаловались
   - Основные данные пользователя (ФИО, ID)
   - Причину жалобы
   - Комментарий жалующегося (если добавил)

### 🔐 Защита от спама

- Максимум 5 жалоб в час от одного пользователя
- Дубликаты жалоб отклоняются
- Проверка существования объекта жалобы

### 📊 Логирование

- Все действия администраторов логируются в `AdminLog`
- Сохраняется: админ ID, действие, цель, деталь, время
- Уведомления отправляются при создании и обновлении жалоб

## Уведомления

### Типы уведомлений

1. **ComplaintCreated** - При создании новой жалобы
   - Отправляется всем администраторам
   - Содержит имя пользователя, на которого жалоба

2. **ComplaintUpdated** - При изменении статуса
   - Отправляется другим администраторам (не тому, кто изменил)
   - Содержит номер жалобы и новый статус

3. **Warning** - При выдаче предупреждения
   - Отправляется пользователю, на которого жалоба была разрешена
   - Сообщает о нарушении правил

## Интеграция с существующими системами

### 1. Integrация с NotificationsService
- Используется для отправки уведомлений администраторам и пользователям

### 2. Integration с AdminService
- Для блокирования пользователей (при разрешении жалобы)
- Для логирования действий

### 3. User статистика
- Можно отслеживать количество жалоб на пользователя
- Данные доступны через `GetComplaintStatsAsync`

## Представления (Views)

### 1. Complaints.cshtml
Страница просмотра всех жалоб:
- Статистика в верхней части (новые, разрешённые, отклонённые)
- Фильтры по статусу
- Таблица со всеми жалобами
- Ссылки на подробный просмотр

### 2. ComplaintDetail.cshtml
Детальный просмотр жалобы:
- Информация о жалобе и сроках
- Данные пользователя, на которого жалоба
- Содержимое спорного контента
- Причина жалобы
- История администратора
- Применённые санкции
- Боковая панель с действиями для изменения статуса

## Миграция БД

Миграция: `20260420170544_AddComplaintsSystem.cs`

Создаёт таблицу `Complaints` с индексами:
- `IX_Complaints_Status` - для быстрого поиска по статусу
- `IX_Complaints_TargetUserId` - для быстрого поиска жалоб на пользователя
- `IX_Complaints_CreatedAt` - для сортировки по дате
- `IX_Complaints_TargetUserId_Status` - для комбинированного поиска

## Примечания по разработке

### Добавленные файлы:

1. **Models:**
   - `/Models/Complaint.cs`

2. **DTO:**
   - `/DTO/Complaints/CreateComplaintRequest.cs`
   - `/DTO/Complaints/ComplaintDto.cs`
   - `/DTO/Complaints/UpdateComplaintStatusRequest.cs`

3. **Services:**
   - `/Services/Interfaces/IComplaintService.cs`
   - `/Services/ComplaintService.cs`

4. **Controllers:**
   - `/Controllers/ComplaintsController.cs` (расширен `/Controllers/AdminController.cs`)

5. **Views:**
   - `/Views/Admin/Complaints.cshtml`
   - `/Views/Admin/ComplaintDetail.cshtml`

6. **ViewModels:**
   - `/ViewModel/Admin/AdminComplaintsViewModel.cs`

7. **Migrations:**
   - `/Migrations/20260420170544_AddComplaintsSystem.cs`
   - `/Migrations/20260420170544_AddComplaintsSystem.Designer.cs`

### Измененные файлы:

1. `/Data/SofiaDbContext.cs` - добавлен DbSet для Complaints
2. `/Models/NotificationType.cs` - добавлены типы ComplaintCreated и ComplaintUpdated
3. `/Program.cs` - зарегистрированы сервисы

## Тестирование

### Сценарий 1: Подача жалобы на сообщение чата

```bash
curl -X POST https://localhost:5001/api/complaints \
  -H "Content-Type: application/json" \
  -d '{
    "targetUserId": "user-2",
    "messageId": "message-guid",
    "reason": 0,
    "reasonText": "Оскорбления"
  }'
```

Expected: 201 Created с данными жалобы

### Сценарий 2: Администратор просматривает жалобы

```bash
curl -X GET https://localhost:5001/api/complaints/admin/all \
  -H "Authorization: Bearer admin-token"
```

Expected: 200 OK с массивом жалоб

### Сценарий 3: Администратор разрешает жалобу с баном

```bash
curl -X PATCH https://localhost:5001/api/complaints/1/status \
  -H "Content-Type: application/json" \
  -d '{
    "status": 2,
    "applyBan": true,
    "adminComment": "Заблокирован за оскорбления"
  }'
```

Expected: 204 No Content

## Будущие улучшения

1. **Автоматическая модерация** - использование ML для автоматического обнаружения проблемного контента
2. **Апелляция жалоб** - система обжалования решений модераторов
3. **Клиентская часть** - UI для добавления функции жалоб в контекстное меню
4. **Расширенные отчеты** - детальные отчеты по модерации за периоды
5. **Интеграция с соцсетями** - для экспортирования данных нарушений

## Лицензия

Sofia App - 2026

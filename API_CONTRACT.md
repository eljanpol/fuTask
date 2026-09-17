# Спецификация: Task Service и Notification service

## 1. Модель данных Task

| Поле| Тип| Описание| Примечания|
|---|---|---|---|
| id| UUID | Уникальный идентификатор задачи | генерируется сервером|
| title| string | Заголовок задачи| |
| description| string | Описание задачи | |
| status|TaskStatus| Статус задачи| значения: new, in_progress, done|
| created_at | DateTime| Момент создания задачи|  |

### Значения поля status
| Значение| Описание|
|---|---|
| new| новая задача|
| in_progress| задача в работе|
| done| задача завершена|


## 2. Эндпойнты (Endpoints)

### 2.1 Task Service — создание задачи
| Элемен| Значение|
|---|---|
| Метод| POST |
| Путь| /api/tasks|
| Входящие данные| JSON объект CreateTask без id и created_at|
| Обязательные поля| title, description|
| Код ответа| 201 Created|
| Тело ответа| полный объект Task (id и created_at присутствуют)|

### 2.2 Notification Service — логирование задачи
| Элемент| Значение|
|---|---|
| Метод| POST|
| Путь| /api/webhooks/task_created|
| Входящие данные| JSON объект Task (полностью)|
| Код ответа| 200|
| Тело ответ| status: 200|

## 3. Формат тела вебхука

| Поле | Тип | Описание |
|---|---|---|
| id| UUID   | Уникальный идентификатор задачи|
| title| string | Заголовок задачи|
| description | string | Описание задачи|
| status| TaskStatus| Статус задачи: new, in_progress, done|
| created_at| DateTime| Дата создания|

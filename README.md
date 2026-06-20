# NetDissector

[![Build and Test](https://github.com/KarpenkoDima/NetDissector/actions/workflows/build.yml/badge.svg)](https://github.com/KarpenkoDima/NetDissector/actions/workflows/build.yml)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

библиотека для парсинга и создания сетевых пакетов на C#. Работает с протоколами уровня 2-4 модели OSI.

## 🎯  О проекте
NetDissector - инструмент для изучения сетевых технологи й через практическую реализацию. Планируется использовть в сетевом анализаторе типа tcpdump.

### Возможности

- 📦 **Парсинг пакетов** из `ReadOnlySpan<byte>` (zero-copy)
- 🔨**Создание пакетов** программно
- 🔄**Сериализация** в Network Byte Order
- ✅**Валидация** структуры пакетов

## ⚡Быстрый старт
### Требования
- .NET 9.0 SDK
### Установка
```bash
git clone https://github.com/KarpenkoDima.NetDissector.git
cd NetDissector
dotnet build
dotnet test
```
### Пример использования
```csharp
using NetDissector.Protocols
//Парсинг Ethernet фрейма
byte[] rawData = new byte[]
{
    0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // Destination MAC (broadcast)
    0x00, 0x0C, 0x29, 0x48, 0x8A, 0x2B, // Source MAC
    0x08, 0x00,                         // EtherType (IPv4)
    0x45, 0x00, 0x00, 0x54              // Payload...
}

if (EthernetFrame.TryParse(rawData, out EthernetFrame frame))
{
    Console.WriteLine($"Source MAC: {BitConverter.ToString(frame.SourceMac.ToArray())}");
    Console.WriteLine($"Dest MAC: {BitConverter.ToString(frame.DestinationMac.ToArray())}");
    Console.WriteLine($"EtherType: 0x{frame.EtherType:X4}");
}
```
**Вывод:**
```
Source MAC: 00-0C-29-48-8A-28
Dest MAC: FF-FF-FF-FF-FF-FF
EtherType: 0x0800
```
## 🏗Архитектура
```
NetDissector/
├── Protocols/
│   ├── EthernetFrame.cs     # Ethernet II (DIX) parser (readonly ref struct, zero-allocation)
│   └── EthernetFields.cs    # Константы структуры фрейма
└── NetDissector.csproj

NetDissector.Tests/
└── EthernetFrameTest.cs     # Unit-тесты
```
### Конвенция парсинга

Каждый протокол реализуется как `readonly ref struct` со статическими методами
`TryParse(ReadOnlySpan<byte>, out T)` и `TrySerialize(Span<byte>, out int bytesWritten)`,
без исключений в качестве управления потоком и без аллокаций в hot path.

## 🗺 Roadmap
### v0.1.0✅
- [x] Базовая архитектура (zero-allocation `TryParse`/`TrySerialize`)
- [x] Ethernet II frame parser
- [x] Unit-тесты
- [x] CI/CD (GitHub Actions)
### v0.2.0
- [ ] ARP protocol
- [ ] IPv4 protocol
- [ ] Улучшенная валидация
### v0.3.0
- [ ] ICMP protocol
- [ ] TCP protocol
- [ ] UDP protocol
- ### v1.0.0
- [ ] Полная поддержка layer 2-4
- [ ] Nuget пакет
- [ ] Интеграция с SharpPcap для захвата трафика
## 🧪Тестирование
```bash
# Зауск тестов
dotnet test

# с подробным выводом
dotnet test --verbosity normal
```
## 🤝 Contributing
1. Fork репозитория
2. Создай feature breach (`git checkout -b feature/NET-XXX-description`)
3. Commit изменений (`git commit -m 'feat(NET-XXX-description): add something'`)
4. Push in branch (`git push origin feature/NET-XXX-description`)
5. Открой Pull Request в `develop`
### Conventional Commits
используем формат: type(scope): description
- `feat` - новая функциональность
- `fix` - исправление бага
- `docs` - документация
- `test` - тесты
- `refactor` - рефакторинг
- `chore` - обслуживание
- `ci` - CI/CD изменения
## 📝Лицензия
MIT License. См. [LICENSE](LICENSE) для деталей.

---

 Сделано с ❤️ для изучения сетевых технологий<
## Префабы смены сцены. ##

1. Разместить на уровне SceneChanger и SceneChangeCollider
2. SceneChanger должен быть на всех игровых сценах и должен быть включен.
3. У SceneChangeCollider указать точное название следующей сцены (использовать индексы сцен
   не предоставляется возможным, т.к Mirror не умеет менять сцену по её индексу, только по имени)
4. Настроить размеры коллайдера.
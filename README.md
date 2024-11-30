Для запуск априложения:
1. Создать таблицы в БД:
```
CREATE TABLE IF NOT EXISTS public.users
(
    "userid" character varying COLLATE pg_catalog."default",
    "username" character varying COLLATE pg_catalog."default",
    "firstname" character varying COLLATE pg_catalog."default",
    "secondname" character varying COLLATE pg_catalog."default",
    "birthdate" character varying COLLATE pg_catalog."default",
    "biography" character varying COLLATE pg_catalog."default",
    "city" character varying COLLATE pg_catalog."default",
    "password" character varying COLLATE pg_catalog."default",
	CONSTRAINT "username_pkey" PRIMARY KEY ("username")
)

CREATE TABLE IF NOT EXISTS public.friends
(
    userid character varying COLLATE pg_catalog."default" NOT NULL,
    friendid character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT friends_pkey PRIMARY KEY (userid, friendid)
)

CREATE TABLE IF NOT EXISTS public.posts
(
    userid character varying COLLATE pg_catalog."default" NOT NULL,
    text character varying COLLATE pg_catalog."default" NOT NULL,
    postid character varying COLLATE pg_catalog."default" NOT NULL,
    postcreatedat timestamp with time zone NOT NULL,
    postupdatedat timestamp with time zone NOT NULL,
    CONSTRAINT posts_pkey PRIMARY KEY (postid)
)

CREATE TABLE IF NOT EXISTS public.dialogs
(
    fromuserid character varying COLLATE pg_catalog."default" NOT NULL,
    touserid character varying COLLATE pg_catalog."default" NOT NULL,
    text character varying COLLATE pg_catalog."default" NOT NULL,
    createdat bigint NOT NULL,
    CONSTRAINT dialogs_pkey PRIMARY KEY (fromuserid, touserid)
)

```
2. Из корневой директории репы собрать образ: ```docker build -t lesson_0 .```
2.1. Из директории DialogService собрать образ сервиса диалогов ```docker build -t dialogs .```
3. Заменить переменные относящиеся к БД и запустить контейнер
```
docker run -it -p 7071:8080 -e "ASPNETCORE_HTTP_PORTS=8080" -e "pgsql_server_master=172.30.64.1" -e "pgsql_port_master=5432" -e "pgsql_server_slave=172.30.64.1" -e "pgsql_port_slave=5434" -e "pgsql_db=lesson_0" -e "pgsql_user=postgres" -e "pgsql_password=password" -e "rabbit_server=172.30.64.1" -e "rabbit_port=5672" -e "rabbit_user=user" -e "rabbit_password=password" -e "dialogs_grpc_url=http://172.30.64.1:7072" -e "ASPNETCORE_ENVIRONMENT=Development" lesson_0
```
3.1. Заменить переменные относящиеся к БД и запустить контейнер сервиса диалогов
```
docker run -it -p 7072:8080 -e "ASPNETCORE_HTTP_PORTS=8080" -e "pgsql_server_master=172.30.64.1" -e "pgsql_port_master=5432" -e "pgsql_server_slave=172.30.64.1" -e "pgsql_port_slave=5434" -e "pgsql_db=lesson_0" -e "pgsql_user=postgres" -e "pgsql_password=password" -e "ASPNETCORE_ENVIRONMENT=Development" dialogs
```
4. Swagger UI доступен по пути /swagger/index.html
5. AsyncAPI UI доступен по пути /asyncapi/ui/index.html
6. Для подключения к сокет серверу ws://host:port/post/feed/posted
7. После подключения к WS, для начала обмена отправить сообщение {"protocol":"json","version":1} <- последний символ обязатлен
8. В RabbitMq создать Exchange posts, остальное автоматически создается и биндится
9. В Redis создать функции:
  - FUNCTION LOAD "#!lua name=otus\nredis.register_function('SendMessage', function(keys, args) return redis.call('XADD', 'dialog:'..keys[1]..':'..keys[2], args[1], 'CreatedAt', args[1], 'Text', args[2]) end)"
  - FUNCTION LOAD "#!lua name=otus2\nredis.register_function('GetDialog', function(keys, args) return redis.call('XREAD', 'STREAMS', 'dialog:'..keys[1]..':'..keys[2], 'dialog:'..keys[2]..':'..keys[1], 0, 0) end)"

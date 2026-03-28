SagaRO2
=======

Saga Ragnarok Online 2 Emulator
-------------------------------

# 28.03.2026

## Модифицирован — SagaGateWay\GatewayClient.cs:

### В OnSendUniversal добавлена ветка p.ServerID == 0x0502

#### Метод OnReturnToCharacterSelect():

##### Вызывает MapSession.Logout(this) — отправляет map-серверу пакет GwLogout (0xFD02), map-сервер выполняет OnLogout → OnDisconnect (сохранение, удаление из мира, выход из пати)

##### Переключает состояние на SESSION_STATE.LOGIN (login-сессия не разрывается — Gateway.Login.Logout(this) НЕ вызывается)

##### Отправляет клиенту AGCT_RECONNECT (ID=0x0208, 11 байт) через gateway framing

##### Вызывает SendCharacterListRequest()

#### Метод SendCharacterListRequest(): формирует inner-пакет CM_CHARLIST ([0x04, 0x00, 0x01, 0x06]) и отправляет через Gateway.Login.SendToLogin. Login-сервер видит opcode 0x0106 (WantCharList) и отвечает списком персонажей.

-------------------------------

##### Bug 1 — SizeIsOk rejects the 10-byte packet (SendUniversal.cs)

#### When fullHeader = true, NetIO routes ALL client packets through the 0xFFFF universal handler (SendUniversal). The SizeIsOk check was size >= 14, but CTGA_RECONNECTLOGIN is only 10 bytes — a bare TCommonHeader with no inner TPacketHeader payload. This caused the "Invalid packet size" error and disconnection.

#### Fix: Changed this.size from 14 to 10. Since isStaticSize() returns false, the dynamic check size >= 10 now accepts the minimum valid gateway packet. All existing packets with inner payloads (0x0301, 0x0501) are always > 14 bytes and are unaffected.

##### Bug 2 — p.ID access crashes on short packets (GatewayClient.cs)

#### Even if the size check passed, the log line p.ID with isFullheader = true reads data[12]/data[13] — bytes that don't exist in a 10-byte packet. This would throw an IndexOutOfRangeException.

#### Fix: Moved the p.ServerID == 0x0502 check before the log line. p.ServerID reads data[6]/data[7] which are always safe for any packet >= 10 bytes. The method returns early for 0x0502, never touching p.ID.

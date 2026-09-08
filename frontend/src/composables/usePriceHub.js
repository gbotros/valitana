import { onMounted, onUnmounted, ref } from "vue";
import { HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { getSignalRUrl, PRICE_UPDATED, toTick } from "../lib/hub";

export function usePriceHub(onTick) {
  const connectionState = ref("disconnected");
  let connection;

  onMounted(async () => {
    connection = new HubConnectionBuilder()
      .withUrl(getSignalRUrl())
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    connection.on(PRICE_UPDATED, (payload) => {
      onTick(toTick(payload));
    });

    connection.onreconnecting(() => {
      connectionState.value = "reconnecting";
    });
    connection.onreconnected(() => {
      connectionState.value = "connected";
    });
    connection.onclose(() => {
      connectionState.value = "disconnected";
    });

    connectionState.value = "connecting";
    try {
      await connection.start();
      connectionState.value = "connected";
    } catch {
      connectionState.value = "disconnected";
    }
  });

  onUnmounted(async () => {
    if (connection && connection.state !== HubConnectionState.Disconnected) {
      await connection.stop();
    }
  });

  return { connectionState };
}

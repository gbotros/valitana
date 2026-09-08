export const PRICE_UPDATED = "PriceUpdated";

export function getSignalRUrl() {
  return window.__VALITANA__?.signalRUrl ?? "http://localhost:5001/hubs/prices";
}

export function toTick(payload) {
  return {
    symbol: payload.symbol,
    price: payload.price,
    occurredAtUtc: payload.occurredAtUtc,
  };
}

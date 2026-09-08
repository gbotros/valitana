export function formatTime(value) {
  return new Date(value).toLocaleTimeString();
}

export function formatPrice(value) {
  return value.toFixed(2);
}

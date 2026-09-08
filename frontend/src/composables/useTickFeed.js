import { computed, ref } from "vue";

const LIST_LIMIT = 12;

export function useTickFeed() {
  const latest = ref(null);
  const ticks = ref([]);
  const direction = ref("");
  let nextId = 0;

  function push(tick) {
    const previousPrice = latest.value?.price ?? null;
    const item = { ...tick, id: ++nextId };

    if (previousPrice != null) {
      direction.value = item.price >= previousPrice ? "up" : "down";
    }

    latest.value = item;
    ticks.value = [item, ...ticks.value].slice(0, LIST_LIMIT);
  }

  const latestClass = computed(() => direction.value);

  return { latest, ticks, latestClass, push };
}

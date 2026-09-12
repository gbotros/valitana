<script setup>
import { onMounted, onUnmounted, ref, watch } from "vue";
import {
  CategoryScale,
  Chart,
  Filler,
  LinearScale,
  LineController,
  LineElement,
  PointElement,
  Tooltip,
} from "chart.js";
import { formatTime } from "../lib/format";

Chart.register(
  LineController,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  Filler,
  Tooltip,
);

const CHART_LIMIT = 60;

const props = defineProps({
  tick: { type: Object, default: null },
});

const canvas = ref(null);
let chart;
let lastId = null;

function cssVar(name) {
  return getComputedStyle(document.documentElement).getPropertyValue(name).trim();
}

function colorWithAlpha(color, alpha) {
  const hex = color.replace("#", "");
  if (hex.length !== 6) {
    return color;
  }

  const n = Number.parseInt(hex, 16);
  const r = (n >> 16) & 255;
  const g = (n >> 8) & 255;
  const b = n & 255;
  return `rgba(${r}, ${g}, ${b}, ${alpha})`;
}

function appendPoint(tick) {
  if (!chart || !tick || tick.id === lastId) {
    return;
  }

  lastId = tick.id;

  chart.data.labels.push(formatTime(tick.occurredAtUtc));
  chart.data.datasets[0].data.push(tick.price);

  if (chart.data.labels.length > CHART_LIMIT) {
    chart.data.labels.shift();
    chart.data.datasets[0].data.shift();
  }

  chart.update("none");
}

onMounted(() => {
  const accent = cssVar("--accent");
  const line = cssVar("--line");
  const muted = cssVar("--muted");

  chart = new Chart(canvas.value, {
    type: "line",
    data: {
      labels: [],
      datasets: [
        {
          label: "Price",
          data: [],
          borderColor: accent,
          backgroundColor: colorWithAlpha(accent, 0.12),
          fill: true,
          tension: 0.25,
          pointRadius: 0,
          borderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      animation: false,
      plugins: {
        legend: { display: false },
      },
      scales: {
        x: {
          ticks: { color: muted, maxTicksLimit: 8 },
          grid: { color: line },
        },
        y: {
          ticks: { color: muted },
          grid: { color: line },
        },
      },
    },
  });

  if (props.tick) {
    appendPoint(props.tick);
  }
});

watch(
  () => props.tick,
  (tick) => {
    appendPoint(tick);
  },
);

onUnmounted(() => {
  chart?.destroy();
});
</script>

<template>
  <div class="chart">
    <canvas ref="canvas"></canvas>
  </div>
</template>

<style lang="scss" scoped>
@use "../styles/tokens" as *;

.chart {
  width: 100%;
  height: $chart-height;
}
</style>

<script setup>
import PriceChart from "./components/PriceChart.vue";
import { usePriceHub } from "./composables/usePriceHub";
import { useTickFeed } from "./composables/useTickFeed";
import { formatPrice, formatTime } from "./lib/format";

const { latest, ticks, latestClass, push } = useTickFeed();
const { connectionState } = usePriceHub(push);
</script>

<template>
  <main>
    <h1>Valitana</h1>
    <p class="sub">Live stock prices over SignalR</p>

    <div class="row">
      <section class="card stat">
        <label>Connection</label>
        <div class="status" :class="connectionState">{{ connectionState }}</div>
      </section>
      <section class="card stat">
        <label>{{ latest?.symbol ?? "—" }}</label>
        <div class="price" :class="latestClass">
          {{ latest ? formatPrice(latest.price) : "—" }}
        </div>
      </section>
    </div>

    <section class="card">
      <PriceChart :tick="latest" />
    </section>

    <section class="card ticks">
      <h2>Recent ticks</h2>
      <ul>
        <li v-for="tick in ticks" :key="tick.id">
          <span>{{ tick.symbol }} {{ formatPrice(tick.price) }}</span>
          <span>{{ formatTime(tick.occurredAtUtc) }}</span>
        </li>
      </ul>
    </section>
  </main>
</template>

<style lang="scss">
@use "./styles/tokens" as *;

h1 {
  margin: 0 0 4px;
  font-size: 1.6rem;
  letter-spacing: 0.04em;
}

.sub {
  margin: 0 0 24px;
  color: $muted;
}

.row {
  display: flex;
  gap: 16px;
  flex-wrap: wrap;
  margin-bottom: 20px;
}

.card {
  background: $panel;
  border: 1px solid $line;
  border-radius: $radius-card;
  padding: 16px 18px;
}

.stat {
  min-width: 160px;

  label {
    @include label;
  }
}

.price {
  font-family: $mono;
  font-size: 2rem;
  line-height: 1.1;

  &.up {
    color: $accent;
  }

  &.down {
    color: $down;
  }
}

.status {
  font-weight: 600;

  &.connected {
    color: $accent;
  }

  &.connecting,
  &.reconnecting {
    color: $warn;
  }

  &.disconnected {
    color: $down;
  }
}

.ticks {
  margin-top: 20px;

  h2 {
    @include label;
  }

  ul {
    list-style: none;
    margin: 0;
    padding: 0;
  }

  li {
    display: flex;
    justify-content: space-between;
    gap: 16px;
    padding: 8px 0;
    border-bottom: 1px solid $line;
    font-family: $mono;
    font-size: 0.9rem;

    &:last-child {
      border-bottom: none;
    }
  }
}
</style>

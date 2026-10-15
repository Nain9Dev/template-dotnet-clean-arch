<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface WorkItem {
  id: string;
  title: string;
  description: string;
  status: 'Todo' | 'InProgress' | 'Done' | 'Archived';
  priority: 'Low' | 'Medium' | 'High' | 'Critical';
  createdAtUtc: string;
  updatedAtUtc: string | null;
}

interface Health {
  status: string;
  service: string;
  framework: string;
  environment: string;
  timestampUtc: string;
}

const health = ref<Health | null>(null);
const items = ref<WorkItem[]>([]);
const newTitle = ref('');
const newDesc = ref('');
const newPriority = ref<'Low' | 'Medium' | 'High' | 'Critical'>('Medium');
const filterStatus = ref<string>('all');
const isLoading = ref(false);
const errorMsg = ref('');

const fetchHealth = async () => {
  try {
    const res = await fetch('/api/v1/health');
    if (res.ok) {
      health.value = await res.json();
    }
  } catch {
    health.value = {
      status: 'offline',
      service: 'template-dotnet-clean-arch',
      framework: '.NET 10',
      environment: 'local',
      timestampUtc: new Date().toISOString()
    };
  }
};

const fetchItems = async () => {
  try {
    const url = filterStatus.value === 'all' 
      ? '/api/v1/work-items' 
      : `/api/v1/work-items?status=${filterStatus.value}`;
    const res = await fetch(url);
    if (res.ok) {
      items.value = await res.json();
    }
  } catch (err) {
    console.error('Failed to load items', err);
  }
};

const createItem = async () => {
  if (!newTitle.value.trim()) return;
  isLoading.value = true;
  errorMsg.value = '';

  try {
    const res = await fetch('/api/v1/work-items', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        title: newTitle.value,
        description: newDesc.value,
        priority: newPriority.value
      })
    });

    if (!res.ok) {
      const err = await res.json();
      errorMsg.value = err.detail || 'Validation error';
      return;
    }

    newTitle.value = '';
    newDesc.value = '';
    newPriority.value = 'Medium';
    await fetchItems();
  } catch (err) {
    errorMsg.value = String(err);
  } finally {
    isLoading.value = false;
  }
};

const advanceStatus = async (item: WorkItem) => {
  const nextStatusMap: Record<WorkItem['status'], WorkItem['status']> = {
    Todo: 'InProgress',
    InProgress: 'Done',
    Done: 'Archived',
    Archived: 'Archived'
  };

  const nextStatus = nextStatusMap[item.status];
  if (nextStatus === item.status) return;

  try {
    const res = await fetch(`/api/v1/work-items/${item.id}/status`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ status: nextStatus })
    });

    if (res.ok) {
      await fetchItems();
    }
  } catch (err) {
    console.error('Failed to update status', err);
  }
};

const deleteItem = async (id: string) => {
  try {
    const res = await fetch(`/api/v1/work-items/${id}`, {
      method: 'DELETE'
    });

    if (res.ok) {
      await fetchItems();
    }
  } catch (err) {
    console.error('Failed to delete item', err);
  }
};

onMounted(async () => {
  await fetchHealth();
  await fetchItems();
});
</script>

<template>
  <div class="shell">
    <header class="hero">
      <div class="container">
        <div class="badge-row">
          <span class="badge" :class="health?.status === 'pass' ? 'badge-good' : 'badge-warn'">
            API: {{ health?.status === 'pass' ? 'Healthy' : (health?.status || 'connecting...') }}
          </span>
          <span class="badge badge-meta">Framework: {{ health?.framework || '.NET 10' }}</span>
          <span class="badge badge-meta">ORM: EF Core 10 + Dapper</span>
          <span class="badge badge-meta">Cost: 0,00 €</span>
        </div>
        <h1 class="title">.NET 10 + Vue 3 Clean Architecture</h1>
        <p class="subtitle">
          Enterprise modular monolith: ASP.NET Core Web API controllers, Domain-Driven invariants, EF Core write model, Dapper fast queries, and PostgreSQL persistence.
        </p>
      </div>
    </header>

    <main class="container content">
      <section class="card create-card">
        <h2>Create Work Item (Domain Invariant Test)</h2>
        <form @submit.prevent="createItem" class="form">
          <div class="form-group">
            <label for="title">Title (1-120 chars)</label>
            <input id="title" v-model="newTitle" placeholder="e.g. Implement OIDC token exchange" required maxlength="120" />
          </div>
          <div class="form-group">
            <label for="desc">Description</label>
            <input id="desc" v-model="newDesc" placeholder="e.g. Support RFC 8693 token exchange pattern" maxlength="500" />
          </div>
          <div class="form-group form-select">
            <label for="priority">Priority</label>
            <select id="priority" v-model="newPriority">
              <option value="Low">Low</option>
              <option value="Medium">Medium</option>
              <option value="High">High</option>
              <option value="Critical">Critical</option>
            </select>
          </div>
          <button type="submit" :disabled="isLoading" class="btn-submit">
            {{ isLoading ? 'Saving...' : 'Add Item' }}
          </button>
        </form>
        <p v-if="errorMsg" class="error" role="alert">{{ errorMsg }}</p>
      </section>

      <section class="card list-card">
        <div class="list-header">
          <div class="list-title-row">
            <h2>Persisted Work Items ({{ items.length }})</h2>
            <select v-model="filterStatus" @change="fetchItems" class="filter-select" aria-label="Filter items by status">
              <option value="all">All Statuses</option>
              <option value="Todo">Todo</option>
              <option value="InProgress">InProgress</option>
              <option value="Done">Done</option>
              <option value="Archived">Archived</option>
            </select>
          </div>
          <button @click="fetchItems" class="btn-refresh" aria-label="Refresh items from database">Refresh</button>
        </div>

        <div v-if="items.length === 0" class="empty">
          No work items found. Add one above to exercise EF Core write persistence and Dapper CQRS query pipeline.
        </div>

        <div v-else class="items-grid">
          <div v-for="item in items" :key="item.id" class="item-card">
            <div class="item-header">
              <span class="item-title">{{ item.title }}</span>
              <span class="priority-pill" :class="`prio-${item.priority.toLowerCase()}`">{{ item.priority }}</span>
            </div>
            <p class="item-desc">{{ item.description || 'No description provided.' }}</p>
            <div class="item-footer">
              <span class="status-pill" :class="`status-${item.status.toLowerCase()}`">{{ item.status }}</span>
              <div class="item-actions">
                <button 
                  v-if="item.status !== 'Archived'" 
                  @click="advanceStatus(item)" 
                  class="btn-advance" 
                  :title="`Advance from ${item.status}`"
                >
                  Advance →
                </button>
                <button @click="deleteItem(item.id)" class="btn-delete" title="Delete work item">✕</button>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  </div>
</template>

<style>
:root {
  --bg: #090c15;
  --surface: #101524;
  --surface-hover: #161c2e;
  --border: rgba(255, 255, 255, 0.08);
  --text: #f1f5f9;
  --text-muted: #94a3b8;
  --accent: #60a5fa;
  --green: #22c55e;
  --yellow: #f59e0b;
  --red: #ef4444;
  --font-sans: 'Inter', system-ui, sans-serif;
  --font-mono: 'JetBrains Mono', monospace;
}

* { box-sizing: border-box; margin: 0; padding: 0; }

body {
  background: var(--bg);
  color: var(--text);
  font-family: var(--font-sans);
  padding-top: 38px;
  line-height: 1.5;
}

.shell { min-height: 100vh; }
.container { max-width: 960px; margin: 0 auto; padding: 0 20px; }

.hero {
  padding: 48px 0 32px;
  border-bottom: 1px solid var(--border);
  background: linear-gradient(180deg, rgba(96, 165, 250, 0.05) 0%, transparent 100%);
}

.badge-row { display: flex; gap: 8px; margin-bottom: 16px; flex-wrap: wrap; }
.badge {
  font-size: 11px;
  font-weight: 600;
  padding: 3px 10px;
  border-radius: 9999px;
  font-family: var(--font-mono);
}
.badge-good { background: rgba(34, 197, 94, 0.15); color: var(--green); border: 1px solid rgba(34, 197, 94, 0.3); }
.badge-warn { background: rgba(239, 68, 68, 0.15); color: var(--red); border: 1px solid rgba(239, 68, 68, 0.3); }
.badge-meta { background: rgba(255, 255, 255, 0.06); color: var(--text-muted); border: 1px solid var(--border); }

.title { font-size: 32px; font-weight: 800; letter-spacing: -0.02em; margin-bottom: 10px; }
.subtitle { font-size: 15px; color: var(--text-muted); max-width: 680px; }

.content { padding-top: 36px; display: flex; flex-direction: column; gap: 28px; padding-bottom: 60px; }

.card {
  background: var(--surface);
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 24px;
}
.card h2 { font-size: 16px; font-weight: 700; margin-bottom: 16px; }

.form { display: grid; grid-template-columns: 2fr 2fr 1fr auto; gap: 14px; align-items: end; }
@media (max-width: 768px) {
  .form { grid-template-columns: 1fr; }
}

.form-group { display: flex; flex-direction: column; gap: 6px; }
.form-group label { font-size: 12px; color: var(--text-muted); font-weight: 500; }
.form-group input, .form-group select, .filter-select {
  background: rgba(0, 0, 0, 0.25);
  border: 1px solid var(--border);
  border-radius: 6px;
  padding: 8px 12px;
  color: #fff;
  font-family: inherit;
  font-size: 14px;
}
.form-group input:focus, .form-group select:focus, .filter-select:focus {
  outline: 2px solid var(--accent);
  border-color: transparent;
}

.btn-submit, .btn-refresh {
  background: var(--accent);
  color: #040914;
  border: none;
  border-radius: 6px;
  padding: 9px 16px;
  font-weight: 600;
  cursor: pointer;
  font-size: 13px;
  transition: opacity 0.15s;
}
.btn-submit:hover, .btn-refresh:hover { opacity: 0.9; }

.btn-refresh {
  background: rgba(255, 255, 255, 0.08);
  color: var(--text);
  border: 1px solid var(--border);
}

.error { color: var(--red); font-size: 13px; margin-top: 10px; }

.list-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; gap: 12px; flex-wrap: wrap; }
.list-title-row { display: flex; align-items: center; gap: 12px; }

.items-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 14px; }
.item-card {
  background: rgba(0, 0, 0, 0.2);
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
.item-header { display: flex; justify-content: space-between; align-items: flex-start; gap: 8px; }
.item-title { font-weight: 600; font-size: 14px; color: #fff; }
.item-desc { font-size: 12px; color: var(--text-muted); flex-grow: 1; }

.priority-pill, .status-pill {
  font-size: 10px;
  font-weight: 700;
  padding: 2px 7px;
  border-radius: 9999px;
  font-family: var(--font-mono);
  text-transform: uppercase;
}
.prio-low { background: rgba(148, 163, 184, 0.2); color: #cbd5e1; }
.prio-medium { background: rgba(96, 165, 250, 0.2); color: var(--accent); }
.prio-high { background: rgba(245, 158, 11, 0.2); color: var(--yellow); }
.prio-critical { background: rgba(239, 68, 68, 0.2); color: var(--red); }

.status-todo { background: rgba(148, 163, 184, 0.15); color: #94a3b8; }
.status-inprogress { background: rgba(96, 165, 250, 0.2); color: #93c5fd; }
.status-done { background: rgba(34, 197, 94, 0.2); color: var(--green); }
.status-archived { background: rgba(255, 255, 255, 0.05); color: #64748b; }

.item-footer { display: flex; justify-content: space-between; align-items: center; margin-top: 8px; }
.item-actions { display: flex; gap: 6px; }

.btn-advance {
  background: rgba(96, 165, 250, 0.15);
  color: var(--accent);
  border: 1px solid rgba(96, 165, 250, 0.3);
  border-radius: 4px;
  padding: 3px 8px;
  font-size: 11px;
  cursor: pointer;
}
.btn-advance:hover { background: rgba(96, 165, 250, 0.25); }

.btn-delete {
  background: transparent;
  color: var(--text-muted);
  border: none;
  font-size: 12px;
  cursor: pointer;
  padding: 2px 6px;
  border-radius: 4px;
}
.btn-delete:hover { color: var(--red); background: rgba(239, 68, 68, 0.1); }

.empty { font-size: 13px; color: var(--text-muted); padding: 32px 0; text-align: center; }
</style>
